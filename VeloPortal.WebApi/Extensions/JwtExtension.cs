using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VeloPortal.Application.Interfaces.Common;
using VeloPortal.Application.Settings;
using VeloPortal.Infrastructure.Service;


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
                options.MapInboundClaims = false; // keep raw claim names: sub, comcod, user_type, user_role
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

            // Authenticated, and the token must carry a known non-vendor user type
            // (tokens issued before this change have no user_type and are rejected -> re-login).
            services.AddAuthorization(o =>
            {
                o.AddPolicy("NonVendorUser", p => p
                    .RequireAuthenticatedUser()
                    .RequireAssertion(ctx =>
                    {
                        var t = ctx.User.FindFirst("user_type")?.Value;
                        return !string.IsNullOrEmpty(t) && t != "vendor";
                    }));
            });

            #region common service
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            #endregion
            return services;

        }
    }
}
