using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VeloPortal.Application.DTOs.Common;
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
        public string GenerateAccessToken(DtoPortalAuthUser? user)
        {
            if (user == null)
            {
                return "";

            }
            else
            {
                var claims = new[]
             {
            new Claim(JwtRegisteredClaimNames.Sub, user.user_id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.user_email??""),
            new Claim(JwtRegisteredClaimNames.Name, user.full_name??"")
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

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
