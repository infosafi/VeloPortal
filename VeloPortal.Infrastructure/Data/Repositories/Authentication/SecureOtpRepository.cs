using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using VeloPortal.Application.Interfaces.Authentication;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;

namespace VeloPortal.Infrastructure.Data.Repositories.Authentication
{
    public class SecureOtpRepository : ISecureOtp
    {
        // Portal-only marker; ERP 2FA rows never use this user_type.
        private const string VendorSignupType = "portal_vendor_signup";
        private const int ExpiryMinutes = 10;
        private const int CooldownSeconds = 60;
        private const int MaxAttempts = 5;
        private static readonly DateTime Unused = new DateTime(1900, 1, 1);

        private readonly IDbContextFactory<VeloPortalLogContext> _factory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SecureOtpRepository> _logger;
        private readonly byte[] _key;

        public SecureOtpRepository(IDbContextFactory<VeloPortalLogContext> factory, IMemoryCache cache,
            IOptions<JwtSettings> jwt, ILogger<SecureOtpRepository> logger)
        {
            _factory = factory;
            _cache = cache;
            _logger = logger;
            // Derived key so the OTP HMAC is independent from the JWT signing key itself
            _key = SHA256.HashData(Encoding.UTF8.GetBytes("otp-hmac:" + (jwt.Value.Secret ?? "")));
        }

        // HMAC(comcod|user|purpose|code) -> base64 (44 chars, fits nvarchar(50)); the plain code is never stored
        private string Hash(string comcod, int userId, string code)
        {
            using var h = new HMACSHA256(_key);
            return Convert.ToBase64String(h.ComputeHash(Encoding.UTF8.GetBytes($"{comcod}|{userId}|{VendorSignupType}|{code}")));
        }

        public async Task<string?> IssueVendorSignupOtpAsync(string comcod, int vendorProfileId, string? ipAddress)
        {
            try
            {
                using var db = _factory.CreateDbContext();
                var now = DateTime.Now;

                // No date comparison inside the SQL (the table uses smalldatetime): fetch this user's latest
                // rows by key only and decide "unused" in memory.
                var latest = await db.SecureLoginInf
                    .Where(x => x.comcod == comcod && x.user_id == vendorProfileId && x.user_type == VendorSignupType)
                    .OrderByDescending(x => x.secure_login_id)
                    .Take(20)
                    .ToListAsync();
                var open = latest.Where(x => x.execution_date == Unused).ToList();

                // resend cooldown
                // (create_date is a smalldatetime, i.e. rounded to the minute, so the effective wait can be up to ~90s)
                if (open.Any(x => x.create_date > now.AddSeconds(-CooldownSeconds)))
                {
                    _logger.LogWarning("Signup OTP NOT issued for vendor {VendorId}: resend cooldown still active", vendorProfileId);
                    return null;
                }

                // a new code invalidates every older unused one
                foreach (var o in open) o.execution_date = now;

                var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
                var ip = ipAddress ?? "";
                db.SecureLoginInf.Add(new SecureLoginInf
                {
                    comcod = comcod,
                    user_id = vendorProfileId,
                    two_factor_otp = Hash(comcod, vendorProfileId, code),
                    create_date = now,
                    expiry_date = now.AddMinutes(ExpiryMinutes),
                    execution_date = Unused,
                    ip_address = ip.Length > 50 ? ip.Substring(0, 50) : ip,
                    user_type = VendorSignupType
                });
                await db.SaveChangesAsync();
                return code;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Issue vendor signup OTP failed");
                return null;
            }
        }

        public async Task<OtpVerifyResult> VerifyVendorSignupOtpAsync(string comcod, int vendorProfileId, string? code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code) || code.Trim().Length != 6 || !code.Trim().All(char.IsDigit))
                    return OtpVerifyResult.Invalid;

                using var db = _factory.CreateDbContext();
                var now = DateTime.Now;

                var latest = await db.SecureLoginInf
                    .Where(x => x.comcod == comcod && x.user_id == vendorProfileId && x.user_type == VendorSignupType)
                    .OrderByDescending(x => x.secure_login_id)
                    .Take(20)
                    .ToListAsync();
                // newest row that has not been used yet (execution_date still 1900-01-01)
                var rec = latest.FirstOrDefault(x => x.execution_date == Unused);

                if (rec == null) return OtpVerifyResult.NotFound;

                if (rec.expiry_date < now)
                {
                    rec.execution_date = now;
                    await db.SaveChangesAsync();
                    return OtpVerifyResult.Expired;
                }

                var cacheKey = "otp-attempts:" + rec.secure_login_id;
                var attempts = _cache.TryGetValue(cacheKey, out int a) ? a : 0;
                if (attempts >= MaxAttempts)
                {
                    rec.execution_date = now; // burn it, a new code must be requested
                    await db.SaveChangesAsync();
                    return OtpVerifyResult.TooManyAttempts;
                }

                var expected = Encoding.UTF8.GetBytes(rec.two_factor_otp);
                var given = Encoding.UTF8.GetBytes(Hash(comcod, vendorProfileId, code.Trim()));
                if (!CryptographicOperations.FixedTimeEquals(expected, given))
                {
                    _cache.Set(cacheKey, attempts + 1, TimeSpan.FromMinutes(ExpiryMinutes + 1));
                    return OtpVerifyResult.Invalid;
                }

                rec.execution_date = now;
                await db.SaveChangesAsync();
                _cache.Remove(cacheKey);
                return OtpVerifyResult.Ok;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Verify vendor signup OTP failed");
                return OtpVerifyResult.Invalid; // fail closed
            }
        }
    }
}
