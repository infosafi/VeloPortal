using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using VeloPortal.Application.Interfaces.Authentication;
using VeloPortal.Application.Interfaces.Documentation;
using VeloPortal.Application.Interfaces.FacilityManagement;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Interfaces.ProjectManagement;
using VeloPortal.Application.Interfaces.Sales;
using VeloPortal.Application.Interfaces.SystemConfig;
using VeloPortal.Application.Interfaces.Vendor;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.Repositories.Authentication;
using VeloPortal.Infrastructure.Data.Repositories.Documentation;
using VeloPortal.Infrastructure.Data.Repositories.FacilityManagement;
using VeloPortal.Infrastructure.Data.Repositories.Procurement;
using VeloPortal.Infrastructure.Data.Repositories.ProjectManagement;
using VeloPortal.Infrastructure.Data.Repositories.Sales;
using VeloPortal.Infrastructure.Data.Repositories.SystemConfig;
using VeloPortal.Infrastructure.Data.Repositories.Vendor;
using VeloPortal.Infrastructure.Service;

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

            // for log DB
            var connectionStringlog = Configuration.GetConnectionString("VelocityLogConnection");

            services.AddDbContextFactory<VeloPortalLogContext>(opt => opt.UseSqlServer(connectionStringlog));

            ////Data Protection
            services.AddDataProtection()
                   // use 14-day lifetime instead of 90-day lifetime
                   .PersistKeysToDbContext<VeloPortalLogContext>()
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

            services.Configure<FtpSettings>(
            Configuration.GetSection("FtpSettings"));
            services.AddScoped<IFtpService, FtpService>();

            #region System configuration Service add
            services.AddScoped<IIndustries, IndustriesRepository>();
            services.AddScoped<IResCodeInf, ResCodeInfRepository>();
            services.AddScoped<IAccCodeInf, AccCodeInfRepository>();
            #endregion

            #region start Project Management Service add
            services.AddScoped<IUnitInfo, UnitInfoRepository>();

            #endregion

            #region Sales Service add
            services.AddScoped<IMoneyRcptPmnt, MoneyReceiptRepository>();
            #endregion

            #region start Authentication Service add
            services.AddScoped<IPortalAuthUser, PortalAuthUserRepository>();
            services.AddScoped<IPassRecovery, PassRecoveryRepository>();
            #endregion

            #region Documentation Service add
            services.AddScoped<IDocInfDet, DocInfDetRepository>();
            #endregion

            #region start Service Request add
            services.AddScoped<IServReqInf, ServReqInfRepository>();
            #endregion

            #region start Vendor add
            services.AddScoped<IVendorSuply, VendorSuplyRepository>();
            #endregion

            #region Procurement Service add
            services.AddScoped<IPurRFQInf, PurRFQInfRepository>();
            #endregion
            return services;
        }
    }
}
