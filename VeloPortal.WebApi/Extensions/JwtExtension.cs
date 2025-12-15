using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VeloPortal.Application.Interfaces.Common;
using VeloPortal.Application.Settings;
using VeloPortal.Infrastructure.Data.Repositories.Common;

namespace VeloPortal.WebApi.Extensions
{
    public static class JwtExtension
    {
        public static IServiceCollection AddJwtServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
                var key = Encoding.UTF8.GetBytes(jwtSettings?.Secret ?? "");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            #region common service
            services.AddScoped<IJwtService, JwtRepository>();
            //services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            #endregion
            return services;

        }
    }
}
