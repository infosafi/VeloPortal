using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;

namespace VeloPortal.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration Configuration)
        {
            // Add services to the container.

            services.AddControllers()
                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep C# casing (PascalCase)
                     // Apply the custom DateTime converter globally
                     options.JsonSerializerOptions.Converters.Add(new DateTimeExtensions.CustomDateTimeConverter());
                 });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            #region DbConnection services start

            var connectionString = Configuration.GetConnectionString("VelocityDBConnection");

            services.AddDbContextFactory<VeloPortalDbContext>(opt => opt.UseSqlServer(connectionString));

            ////Data Protection
            services.AddDataProtection()
                   // use 14-day lifetime instead of 90-day lifetime
                   .PersistKeysToDbContext<VeloPortalDbContext>()
                   .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

        
            #endregion DbConnection service end

            // Cross-Origin Resource Sharing (CORS)
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });

                //options.AddPolicy("AllowSpecific",
                //        builder =>
                //        {
                //            builder.WithOrigins(
                //                "https://itechvelocity.com",
                //                "https://erp.itechvelocity.com"
                //            )
                //            .AllowAnyHeader()
                //            .AllowAnyMethod();
                //        });
            });


   

            return services;
        }
    }
}
