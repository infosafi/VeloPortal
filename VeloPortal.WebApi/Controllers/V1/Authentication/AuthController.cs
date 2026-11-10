using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using VeloPortal.Application.DTOs.Authentication;
using VeloPortal.Application.Interfaces.Authentication;
using VeloPortal.Application.Interfaces.Common;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Entities.SystemConfig;
using VeloPortal.Domain.Enums;
using VeloPortal.Domain.Extensions;
using VeloPortal.WebApi.Helpers;
using static Azure.Core.HttpHeader;

namespace VeloPortal.WebApi.Controllers.V1.Authentication
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshRepo;
        private readonly IPortalAuthUser _userRepo;
        private readonly IPassRecovery _passRecovery;
        private readonly IVendorProfile _vendorprofile;
        private readonly ILoginLogs _loginlogs;
        private readonly ISecureOtp _secureOtp;
        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;
        private readonly byte[] _otpKey;

        public AuthController(IJwtService jwtService, IRefreshTokenService refreshRepo, ILoginLogs loginlogs,
            IPortalAuthUser userRepo, IPassRecovery passRecovery, IVendorProfile vendorprofile, ISecureOtp secureOtp,
            Microsoft.Extensions.Caching.Memory.IMemoryCache cache, Microsoft.Extensions.Options.IOptions<JwtSettings> jwtOptions)
        {
            _secureOtp = secureOtp;
            _cache = cache;
            // derived key: OTP HMAC is independent of the JWT signing key itself
            _otpKey = SHA256.HashData(Encoding.UTF8.GetBytes("otp-hmac:" + (jwtOptions.Value.Secret ?? "")));
            _jwtService = jwtService;
            _refreshRepo = refreshRepo;
            _loginlogs = loginlogs;
            _userRepo = userRepo;
            _passRecovery = passRecovery;
            _vendorprofile = vendorprofile;
        }


        [HttpPost("portal-login")]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
        public async Task<IActionResult> Login(DtoPortalAuthUser dto)
        {
            if (dto.comcod?.Length == 0)
            {
                return BadRequest(new { Success = false, message = "Company Selection is missing!" });
            }
            if (dto.user_or_email?.Length == 0)
            {
                return BadRequest(new { Success = false, message = "Username or Email Mandatory!" });
            }
            if (dto.password?.Length == 0)
            {
                return BadRequest(new { Success = false, message = "Password Mandatory!" });
            }

            string encpassword = EncryptionExtension.PasswordEnc(dto.password ?? "");

            var user = await _userRepo.ValidateCredentialsAsync(dto.comcod ?? "", dto.user_type ?? "", dto.user_or_email ?? "", encpassword);

            if (user == null)
            {
                await LogLoginAttemptAsync(dto, 0, false, "Invalid credentials", "");
                return Unauthorized(new { Success = false, message = "User or Password Invalid", twofactor = false });
            }
            if (user.unq_id == 0)
            {
                await LogLoginAttemptAsync(dto, 0, false, "Invalid credentials", "");
                return Unauthorized(new { Success = false, message = "User or Password Invalid", twofactor = false });
            }

            // Vendors self-register: only e-mail verified, non-held vendors may obtain a token.
            if (string.Equals(dto.user_type?.Trim(), "vendor", StringComparison.OrdinalIgnoreCase))
            {
                if (!await _vendorprofile.IsVendorLoginAllowedAsync(dto.comcod ?? "", user.unq_id))
                {
                    await LogLoginAttemptAsync(dto, user.unq_id, false, "Vendor email not verified or on hold", "");
                    return Unauthorized(new { Success = false, message = "Account email is not verified or the account is on hold.", twofactor = false });
                }
            }

            var accessToken = _jwtService.GenerateAccessToken(user, dto.user_type);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _refreshRepo.SaveAsync(new RefreshToken
            {
                token = refreshToken,
                expires = DateTime.UtcNow.AddDays(7),
                user_id = user.unq_id
            });

            string sessionid = GenerateSessionId(user.unq_id, dto.comcod ?? "");

            await LogLoginAttemptAsync(dto, user.unq_id, true, "", sessionid);

            return Ok(new
            {
                accessToken,
                refreshToken,
                userinfo = user,
                Success = true,
                message = "Successfully Logged In"
            });
        }


        /// <summary>Real client address as seen by the server (the client-sent value can be empty or forged).
        /// Behind a reverse proxy the right-most X-Forwarded-For entry (added by our own proxy) is used.</summary>
        private string ServerClientIp()
        {
            var xff = Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrWhiteSpace(xff))
            {
                var last = xff.Split(',').Select(s => s.Trim()).LastOrDefault(s => s.Length > 0);
                if (!string.IsNullOrEmpty(last)) return Clip(last, 45);
            }
            return Clip(HttpContext.Connection.RemoteIpAddress?.ToString(), 45);
        }

        /// <summary>Writes a login attempt (success or failure) with server-side IP and User-Agent.</summary>
        private async Task LogLoginAttemptAsync(DtoPortalAuthUser dto, int userId, bool ok, string reason, string sessionId)
        {
            try
            {
                var logs = new LoginLogs
                {
                    userid = userId,
                    comcod = Clip(dto.comcod, 5),
                    username_or_email = Clip(dto.user_or_email, 100),
                    login_status = ok,
                    failure_reason = Clip(reason, 100),
                    ip_address = ServerClientIp(),
                    user_agent = Clip(Request.Headers.UserAgent.ToString(), 250),
                    mac_address = dto.macaddress ?? "",
                    remarks = dto.user_type ?? "",
                    usession_id = sessionId,
                    location = dto.location ?? "",
                    attempt_time = DateTimeExtensions.GetLocalTimeFromBaseOnTimeZone()
                };
                await _loginlogs.InsertOrUpdateLoginLogs(logs, HelperEnums.Action.Add.ToString());
            }
            catch
            {
                // logging must never break the login flow
            }
        }

        public static string GenerateSessionId(int userId, string comcod)
        {
            // Generate a random number
            byte[] randomBytes = new byte[16];
            RandomNumberGenerator.Fill(randomBytes);
            string randomString = Convert.ToBase64String(randomBytes).Replace("/", "_").Replace("+", "-");

            // Combine with userId and comcod
            string input = $"{randomString}{userId}{comcod}";

            // Hash to create a unique, fixed-length SessionID
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hashBytes).Replace("/", "_").Replace("+", "-").Substring(0, 16);
            }
        }


        [HttpPost("refresh")]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
        public async Task<IActionResult> Refresh(DtoJwtToken dto)
        {
            var storedToken = await _refreshRepo.GetByTokenAsync(dto.RefreshToken);

            if (storedToken == null || storedToken.is_revoked || storedToken.expires < DateTime.UtcNow)
                return Unauthorized();

            // Identity (company / user type / role) must come from the originally issued token,
            // not from client-supplied fields, otherwise a refresh token can be replayed as another user type.
            var oldPrincipal = _jwtService.ReadExpiredToken(dto.AccessToken);
            if (oldPrincipal == null ||
                oldPrincipal.FindFirst("sub")?.Value != storedToken.user_id.ToString())
                return Unauthorized();

            var tokComcod = oldPrincipal.FindFirst("comcod")?.Value ?? "";
            var tokUserType = oldPrincipal.FindFirst("user_type")?.Value ?? "";
            var tokRole = oldPrincipal.FindFirst("user_role")?.Value ?? "";

            var user = await _userRepo.GetUserInfoByIdRole(tokComcod, tokUserType, Convert.ToString(storedToken.user_id), tokRole);
            if (user != null && tokUserType == "vendor" &&
                !await _vendorprofile.IsVendorLoginAllowedAsync(tokComcod, user.unq_id))
                return Unauthorized();
            var newAccessToken = _jwtService.GenerateAccessToken(user, tokUserType);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            storedToken.is_revoked = true;
            await _refreshRepo.RevokeAsync(dto.RefreshToken);

            if (user != null)
            {
                await _refreshRepo.SaveAsync(new RefreshToken
                {
                    token = newRefreshToken,
                    expires = DateTime.UtcNow.AddDays(7),
                    user_id = user.unq_id
                });

                return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
            }
            else
            {
                return Ok(new { message = "Token not refreshed" });
            }
        }


        [HttpGet("get-company-list")]
        public async Task<IActionResult> GetAllCompanyInfoList(bool? is_active)
        {
            var response = await _userRepo.GetCompanyInfoListByStatus(is_active);

            if (response == null)
                return NotFound(ApiResponse<ModuleInf>.FailureResponse(
                    new List<string> { "Company not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            if (response.Count() == 0)
                return NotFound(ApiResponse<IEnumerable<CompanyInf>>.SuccessResponse(response, message: "No Data Found"));

            return Ok(ApiResponse<IEnumerable<CompanyInf>>.SuccessResponse(response, message: ""));
        }

        /// <summary>
        /// Registers a new vendor into the portal.
        /// </summary>
        [HttpPost("portal-signup")]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
        public async Task<IActionResult> Signup(DtoPortalSignup dto)
        {
            // 1. Basic Validation
            if (string.IsNullOrWhiteSpace(dto.comcod))
                return BadRequest(new { Success = false, message = "Company Code is missing!" });

            if (string.IsNullOrWhiteSpace(dto.email) || string.IsNullOrWhiteSpace(dto.password))
                return BadRequest(new { Success = false, message = "Email and Password are required!" });

            if (dto.password.Length < 6)
                return BadRequest(new { Success = false, message = "Password must be at least 6 characters." });


            if (string.IsNullOrWhiteSpace(dto.company_name))
                return BadRequest(new { Success = false, message = "Company Name is required!" });

            // Input hardening: format/length checks and no markup (stored-XSS / junk accounts)
            dto.email = dto.email.Trim();
            dto.company_name = dto.company_name.Trim();
            dto.phone = (dto.phone ?? "").Trim();
            if (dto.email.Length > 100 || !System.Text.RegularExpressions.Regex.IsMatch(dto.email, @"^[^@\s<>""']+@[^@\s<>""']+\.[^@\s<>""']+$"))
                return BadRequest(new { Success = false, message = "A valid email address is required." });
            if (dto.password.Length > 100)
                return BadRequest(new { Success = false, message = "Password is too long." });
            if (dto.company_name.Length > 200 || dto.company_name.IndexOfAny(new[] { '<', '>' }) >= 0)
                return BadRequest(new { Success = false, message = "Company name is invalid." });
            if (dto.phone.Length > 20 || !System.Text.RegularExpressions.Regex.IsMatch(dto.phone, @"^[0-9+\-\s()]*$"))
                return BadRequest(new { Success = false, message = "Phone number is invalid." });
            if (dto.comcod.Length > 10 || !dto.comcod.All(char.IsDigit))
                return BadRequest(new { Success = false, message = "Company Code is invalid." });

            // Public signup can only ever create a vendor, regardless of what the client sends.
            dto.user_type = "vendor";

            try
            {

                var existingUser = await _userRepo.FindUserByEmailOrPhoneAsync(dto.comcod, dto.user_type, dto.email);
                if (existingUser != null && existingUser.unq_id != 0)
                {
                    return BadRequest(new { Success = false, message = "User with this email already exists." });
                }

                var newVendor = new VendorProfile
                {
                    comcod = dto.comcod,
                    company_name = dto.company_name ?? "Shaha Cement",
                    vendor_email = dto.email,
                    contact_no = dto.phone,
                    vendor_password = EncryptionExtension.PasswordEnc(dto.password), 
                    created_date = DateTime.Now,
                    is_available = true,
                    is_verify_acc = false,
                    is_approved = false,
                    is_hold = false,
                    is_audit = false
                };

                var result = await _vendorprofile.InsertOrUpdateVendor(newVendor, HelperEnums.Action.Add.ToString());

                if (result == null)
                {
                    return BadRequest(new { Success = false, message = "Registration failed." });
                }

                // Verify the e-mail address: send a one-time code. The vendor stays unverified until it is confirmed.
                await SendVendorSignupOtpAsync(dto.comcod, (int)result.vendor_profile_id, dto.email, dto.company_name);

                // Never echo the stored profile (it contains the password hash) back to the caller.
                return Ok(new { Success = true, email_verification_required = true,
                    message = "Registration successful! A verification code was sent to your email. After verifying, please wait for account approval." });

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Success = false, message = "An error occurred during signup." });
            }
        }

        private async Task<bool> SendVendorSignupOtpAsync(string comcod, int vendorProfileId, string email, string? companyName)
        {
            var code = await _secureOtp.IssueVendorSignupOtpAsync(comcod, vendorProfileId, HttpContext.Connection.RemoteIpAddress?.ToString());
            if (code == null) return false;

            var name = System.Net.WebUtility.HtmlEncode(companyName ?? "");
            var body = $@"<div style=""font-family:Arial,sans-serif;max-width:480px;margin:auto"">
<h2>VeloPortal email verification</h2>
<p>Hello <strong>{name}</strong>, use this code to verify your email. It is valid for 10 minutes.</p>
<p style=""font-size:36px;letter-spacing:8px;font-weight:bold"">{code}</p>
<p style=""color:#777"">If you did not sign up, ignore this email.</p></div>";
            await EmailHelper.SendEmail(email, "Verify your VeloPortal email", body);
            return true;
        }

        /// <summary>Confirms the vendor's email with the code that was e-mailed at signup.</summary>
        [HttpPost("verify-signup-otp")]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifySignupOtp([FromBody] DtoVerifySignupOtp dto)
        {
            // One identical answer for every failure (unknown e-mail, already verified, wrong/expired code, too many
            // attempts) so the endpoint cannot be used to find out which e-mail addresses have a vendor account.
            var generic = BadRequest(new { Success = false, message = "Invalid or expired verification code. If needed, request a new code." });
            if (string.IsNullOrWhiteSpace(dto.comcod) || string.IsNullOrWhiteSpace(dto.email) || string.IsNullOrWhiteSpace(dto.otp))
                return generic;

            var vendor = await _vendorprofile.FindVendorByEmailAsync(dto.comcod.Trim(), dto.email.Trim());
            if (vendor == null || vendor.is_email_verify == true) return generic;

            var res = await _secureOtp.VerifyVendorSignupOtpAsync(vendor.comcod, (int)vendor.vendor_profile_id, dto.otp);
            if (res != OtpVerifyResult.Ok) return generic;

            if (!await _vendorprofile.SetEmailVerifiedAsync(vendor.comcod, vendor.vendor_profile_id))
                return StatusCode(500, new { Success = false, message = "Could not complete verification. Please try again." });

            return Ok(new { Success = true, message = "Email verified. You can now log in." });
        }

        /// <summary>Sends a new signup verification code (60s cooldown). Always answers the same way.</summary>
        [HttpPost("resend-signup-otp")]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendSignupOtp([FromBody] DtoResendSignupOtp dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.comcod) && !string.IsNullOrWhiteSpace(dto.email))
            {
                var vendor = await _vendorprofile.FindVendorByEmailAsync(dto.comcod.Trim(), dto.email.Trim());
                if (vendor != null && vendor.is_email_verify != true && !string.IsNullOrEmpty(vendor.vendor_email))
                    await SendVendorSignupOtpAsync(vendor.comcod, (int)vendor.vendor_profile_id, vendor.vendor_email, vendor.company_name);
            }
            // Same reply whether or not the account exists (no user enumeration)
            return Ok(new { Success = true, message = "If the account exists and is not verified, a new code has been sent." });
        }

        /// <summary>
        /// Retrieves the full profile of a customer by email.
        /// </summary>
        [HttpGet("get-customer-profile")]
        [Authorize]
        public async Task<IActionResult> GetCustomerProfile(string comcod, string cust_email)
        {
            var user = await _userRepo.FindUserByCustomerEmailAsync(comcod, "Customer", cust_email);
            if (user == null)
                return NotFound(new { Success = false, message = "Profile not found." });

            return Ok(new { Success = true, data = user });
        }

        /// <summary>
        /// Updates the customer profile details.
        /// </summary>
        [HttpPost("update-customer-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateCustomerProfile(DtoCustomer dto)
        {

            if (dto == null || dto.sup_user_id == 0)
                return BadRequest(new { Success = false, message = "Invalid profile data. User ID is required." });


            if (string.IsNullOrEmpty(dto.user_role))
                return BadRequest(new { Success = false, message = "User role is required for routing." });

            long resultId = await _userRepo.InsertOrUpdateCustomer(dto);

            if (resultId > 0)
            {
                return Ok(new { Success = true, message = "Profile updated successfully!", id = resultId });
            }

            return StatusCode(500, new { Success = false, message = "Failed to update profile. Please check database logs." });
        }

        // ---------------- Password recovery (OTP) ----------------
        // The OTP is bound to the user (id + e-mail), stored only as an HMAC, single use, expires,
        // and limited to a few wrong guesses. Public answers never reveal whether an account exists.
        private const int RecoveryMaxAttempts = 5;
        private const int RecoveryCooldownSeconds = 60;
        private const string RecoveryGenericError = "Invalid or expired code.";

        private string RecoveryHash(int userId, string email, string code)
        {
            using var h = new HMACSHA256(_otpKey);
            return Convert.ToBase64String(h.ComputeHash(
                Encoding.UTF8.GetBytes($"recovery|{userId}|{email.Trim().ToLowerInvariant()}|{code}")));
        }

        private static string Clip(string? s, int max) => string.IsNullOrEmpty(s) ? "" : (s.Length > max ? s.Substring(0, max) : s);

        private async Task IssueRecoveryOtpAsync(string? comcod, string? userType, string? userOrEmail, string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(comcod) || string.IsNullOrWhiteSpace(userOrEmail) || userOrEmail.Length > 100)
                return;

            var user = await _userRepo.FindUserByEmailOrPhoneAsync(comcod, userType ?? "", userOrEmail);
            if (user == null || user.unq_id == 0 || string.IsNullOrWhiteSpace(user.user_email))
                return;

            var now = DateTime.UtcNow;
            var last = await _passRecovery.GetLatestForUserAsync(user.unq_id, user.user_email);
            if (last != null && !last.is_expired)
            {
                if (last.created_date > now.AddSeconds(-RecoveryCooldownSeconds))
                    return; // resend cooldown

                last.is_expired = true; // a new code supersedes the old one
                await _passRecovery.InsertOrUpdatePassRecovery(last, HelperEnums.Action.Update.ToString());
            }

            var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var saved = await _passRecovery.InsertOrUpdatePassRecovery(new PassRecovery
            {
                user_id = user.unq_id,
                user_email = user.user_email,
                recovery_otp = RecoveryHash(user.unq_id, user.user_email, otp),
                created_date = now,
                expiry_date = now.AddMinutes(5),
                execution_date = new DateTime(1900, 1, 1),
                request_agent = Clip(userAgent, 250),
                ip_address = Clip(HttpContext.Connection.RemoteIpAddress?.ToString(), 50),
                is_recovered = false,
                is_expired = false,
                portal_role = !string.IsNullOrEmpty(user.user_role) ? Clip(user.user_role, 7) : "51"
            }, HelperEnums.Action.Add.ToString());
            if (!saved) return;

            var name = System.Net.WebUtility.HtmlEncode(user.fullname ?? "");
            var message = $@"<div style=""font-family:Arial,sans-serif;max-width:480px;margin:auto"">
<h2>VeloPortal password reset</h2>
<p>Hello <strong>{name}</strong>, use this code to reset your password. It is valid for 5 minutes.</p>
<p style=""font-size:36px;letter-spacing:8px;font-weight:bold"">{otp}</p>
<p style=""color:#777"">If you did not request this, you can safely ignore this email. No changes will be made to your account.</p></div>";
            await EmailHelper.SendEmail(user.user_email, "Your VeloPortal Verification Code", message);
        }

        /// <summary>Checks user + code. alreadyVerified=false for verify-otp, true for reset-password.</summary>
        private async Task<(DtoUserInf? user, PassRecovery? rec, string? error)> ValidateRecoveryAsync(
            string? comcod, string? userType, string? userOrEmail, string? otp, bool alreadyVerified)
        {
            if (string.IsNullOrWhiteSpace(comcod) || string.IsNullOrWhiteSpace(userOrEmail) || userOrEmail.Length > 100 ||
                string.IsNullOrWhiteSpace(otp) || otp.Trim().Length != 6 || !otp.Trim().All(char.IsDigit))
                return (null, null, RecoveryGenericError);

            var user = await _userRepo.FindUserByEmailOrPhoneAsync(comcod, userType ?? "", userOrEmail);
            if (user == null || user.unq_id == 0 || string.IsNullOrWhiteSpace(user.user_email))
                return (null, null, RecoveryGenericError);

            var rec = await _passRecovery.GetLatestForUserAsync(user.unq_id, user.user_email);
            if (rec == null || rec.is_expired || rec.expiry_date < DateTime.UtcNow || rec.is_recovered != alreadyVerified)
                return (null, null, RecoveryGenericError);

            var cacheKey = "pr-attempts:" + rec.recover_id;
            var attempts = _cache.TryGetValue(cacheKey, out int a) ? a : 0;
            if (attempts >= RecoveryMaxAttempts)
            {
                rec.is_expired = true; // burn it; a new code must be requested
                await _passRecovery.InsertOrUpdatePassRecovery(rec, HelperEnums.Action.Update.ToString());
                return (null, null, "Too many wrong attempts. Please request a new code.");
            }

            var expected = Encoding.UTF8.GetBytes(rec.recovery_otp ?? "");
            var given = Encoding.UTF8.GetBytes(RecoveryHash(user.unq_id, user.user_email, otp.Trim()));
            if (!CryptographicOperations.FixedTimeEquals(expected, given))
            {
                _cache.Set(cacheKey, attempts + 1, TimeSpan.FromMinutes(15));
                return (null, null, RecoveryGenericError);
            }
            return (user, rec, null);
        }

        /// <summary>
        /// Sends a 6-digit OTP to the user's email for password recovery.
        /// Always answers the same way, whether or not the account exists.
        /// </summary>
        [HttpPost("forgot-password")]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] DtoUserForgotPassword dto)
        {
            if (string.IsNullOrWhiteSpace(dto.user_or_email))
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Phone or Email is required." }, "Phone or Email is required."));

            await IssueRecoveryOtpAsync(dto.comcod, dto.user_type, dto.user_or_email, dto.user_agent);
            return Ok(ApiResponse<bool>.SuccessResponse(true, message: "If the account exists, an OTP has been sent to the registered email address."));
        }

        /// <summary>Verifies the 6-digit OTP (bound to the user). Required before reset-password.</summary>
        [HttpPost("verify-otp")]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] DtoVerifyOtp dto)
        {
            var (user, rec, error) = await ValidateRecoveryAsync(dto.comcod, dto.user_type, dto.user_or_email, dto.otp, alreadyVerified: false);
            if (rec == null)
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { error ?? RecoveryGenericError }, "Invalid OTP."));

            rec.is_recovered = true;
            rec.execution_date = DateTime.UtcNow;
            rec.expiry_date = DateTime.UtcNow.AddMinutes(5); // window to set the new password
            await _passRecovery.InsertOrUpdatePassRecovery(rec, HelperEnums.Action.Update.ToString());

            return Ok(ApiResponse<bool>.SuccessResponse(true, "OTP verified successfully. You can now reset your password."));
        }

        /// <summary>Resets the password. Needs a code that was verified for this same user via verify-otp.</summary>
        [HttpPost("reset-password")]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] DtoResetPassword dto)
        {
            if (string.IsNullOrWhiteSpace(dto.new_password) || string.IsNullOrWhiteSpace(dto.confirm_password))
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "All fields are required." }, "Invalid input."));
            if (dto.new_password != dto.confirm_password)
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Passwords do not match." }, "Validation error."));
            if (dto.new_password.Length < 6 || dto.new_password.Length > 100)
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Password must be 6 to 100 characters." }, "Validation error."));

            var (user, rec, error) = await ValidateRecoveryAsync(dto.comcod, dto.user_type, dto.user_or_email, dto.otp, alreadyVerified: true);
            if (user == null || rec == null)
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { error ?? RecoveryGenericError }, "Invalid token."));

            var role = user.user_role ?? "";
            string userPrefix = role.StartsWith("99") ? "99" : role.StartsWith("15") ? "15" : "51";

            string encryptedPassword = EncryptionExtension.PasswordEnc(dto.new_password);
            bool isUpdateSuccess = await _userRepo.UpdatePasswordAsync(dto.comcod!, dto.user_type ?? "", user.unq_id.ToString(), encryptedPassword, userPrefix);
            if (!isUpdateSuccess)
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Failed to update password in database." }, "Database Error."));

            rec.is_expired = true; // single use
            rec.expiry_date = DateTime.UtcNow;
            await _passRecovery.InsertOrUpdatePassRecovery(rec, HelperEnums.Action.Update.ToString());

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Password has been reset successfully."));
        }

        /// <summary>Sends a new OTP (60s cooldown). Always answers the same way.</summary>
        [HttpPost("resend-otp")]
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] DtoResendOtp dto)
        {
            if (string.IsNullOrWhiteSpace(dto.user_or_email))
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Email is required." }, "Invalid input."));

            await IssueRecoveryOtpAsync(dto.comcod, dto.user_type, dto.user_or_email, dto.user_agent);
            return Ok(ApiResponse<string>.SuccessResponse("If the account exists, a new OTP has been sent.", "OTP resent successfully."));
        }


        /// <summary>
        /// For Change Password of User 
        /// </summary>
        /// <param name="dto">User Info</param>         
        /// <returns>Pasword update</returns>
        [HttpPost("Change-Password")]
        [Authorize]
        public async Task<IActionResult> UserChangePassword(DtoUserChangePassword dto)
        {
            // Who is changing the password comes from the token ONLY (never from the request body),
            // otherwise any logged-in user could set any other user's password.
            var tokUserId = User.FindFirst("sub")?.Value ?? "";
            var tokComcod = User.FindFirst("comcod")?.Value ?? "";
            var tokType = User.FindFirst("user_type")?.Value ?? "";
            var tokRole = User.FindFirst("user_role")?.Value ?? "";
            var tokEmail = User.FindFirst("email")?.Value ?? "";
            var tokName = User.FindFirst("name")?.Value ?? "";
            if (tokUserId == "" || tokComcod == "" || tokType == "")
                return Unauthorized(new { Success = false, message = "Please log in again." });

            if (string.IsNullOrEmpty(dto.old_password) || string.IsNullOrEmpty(dto.new_password))
                return BadRequest(new { Success = false, message = "Old and new password are mandatory!" });
            if (dto.new_password.Length < 6 || dto.new_password.Length > 100)
                return BadRequest(new { Success = false, message = "Password must be 6 to 100 characters." });

            // user type exactly as the user store expects it (e.g. Vendor / Customer)
            var userType = char.ToUpperInvariant(tokType[0]) + tokType.Substring(1);

            // The old password must be correct for THIS token's user.
            var oldEnc = EncryptionExtension.PasswordEnc(dto.old_password);
            DtoUserInf? verified = null;
            foreach (var login in new[] { tokEmail, tokName })
            {
                if (string.IsNullOrWhiteSpace(login)) continue;
                verified = await _userRepo.ValidateCredentialsAsync(tokComcod, userType, login, oldEnc);
                if (verified != null && verified.unq_id.ToString() == tokUserId) break;
                verified = null;
            }
            if (verified == null)
                return BadRequest(new { Success = false, message = "Old password is incorrect." });

            string userPrefix = tokRole.StartsWith("99") ? "99" : tokRole.StartsWith("15") ? "15" : "51";
            string encryptedPassword = EncryptionExtension.PasswordEnc(dto.new_password);

            bool isUpdateSuccess = await _userRepo.UpdatePasswordAsync(tokComcod, userType, tokUserId, encryptedPassword, userPrefix);
            if (!isUpdateSuccess)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(
                   new List<string> { "Failed to update password in database." }, "Database Error."));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Password Updated Successfully."));
        }
    }
}
