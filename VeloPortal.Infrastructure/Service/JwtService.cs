using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VeloPortal.Application.DTOs.Authentication;
using VeloPortal.Application.Interfaces.Common;
using VeloPortal.Application.Settings;

namespace VeloPortal.Infrastructure.Service
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;
        public JwtService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }
        public string GenerateAccessToken(DtoUserInf? user, string? userType = null)
        {
            if (user == null)
            {
                return "";

            }
            else
            {
                var claims = new[]
             {
            new Claim(JwtRegisteredClaimNames.Sub, user.unq_id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.user_email??""),
            new Claim(JwtRegisteredClaimNames.Name, user.username??""),
            // server-issued identity used for authorization (never trust client-sent values)
            new Claim("comcod", user.comcod ?? ""),
            new Claim("user_type", (userType ?? "").Trim().ToLowerInvariant()),
            new Claim("user_role", user.user_role ?? "")
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret ?? ""));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _settings.Issuer,
                    audience: _settings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

        }

        public System.Security.Claims.ClaimsPrincipal? ReadExpiredToken(string? accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken)) return null;
            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret ?? ""))
                };
                var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
                var principal = handler.ValidateToken(accessToken, parameters, out var validated);
                if (validated is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                    return null;
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
