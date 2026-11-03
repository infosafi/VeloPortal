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
    public class LoginLogsRepository : ILoginLogs
    {
        private readonly IDbContextFactory<VeloPortalLogContext> _dbContextFactory;
        private readonly ILogger<LoginLogsRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public LoginLogsRepository(IDbContextFactory<VeloPortalLogContext> dbContextFactory, ILogger<LoginLogsRepository> logger, IMapper mapper, IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);

        }
        public async Task<bool> InsertOrUpdateLoginLogs(LoginLogs obj, string action)
        {
            try
            {

                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    if (action == HelperEnums.Action.Add.ToString())
                    {
                        await dbContext.LoginLogs.AddAsync(obj);
                    }
                    else
                    {
                        dbContext.LoginLogs.Update(obj);
                    }

                    await dbContext.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "User Login Logs Information Saved Failed");
                return false;
            }
        }
    }
}
