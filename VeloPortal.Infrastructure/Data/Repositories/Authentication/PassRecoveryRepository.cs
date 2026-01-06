using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.Interfaces.Authentication;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Enums;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Authentication
{
    public class PassRecoveryRepository : IPassRecovery
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<PassRecoveryRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;
        public PassRecoveryRepository(
         IDbContextFactory<VeloPortalDbContext> dbContextFactory,
         ILogger<PassRecoveryRepository> logger,
         IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<bool> InsertOrUpdatePassRecovery(PassRecovery obj, string action)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    if (action == HelperEnums.Action.Add.ToString())
                    {
                        await dbContext.PassRecovery.AddAsync(obj);
                    }
                    else
                    {
                        dbContext.PassRecovery.Update(obj);
                    }

                    await dbContext.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Pass Recovery Saved Failed");
                return false;
            }
        }

        public async Task<PassRecovery?> GetLatestOtpAsync(string? otp)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    return await dbContext.PassRecovery.FirstOrDefaultAsync(p => p.recovery_otp == otp);

                }
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Latest Otp Retrival Failed");
                return null;
            }

        }
    }
}
