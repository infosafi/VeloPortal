using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using VeloPortal.Application.Interfaces.SystemConfig;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.SystemConfig
{
    public class AppManagementRepository : IAppManagement
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<AppManagementRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public AppManagementRepository(IDbContextFactory<VeloPortalDbContext> dbContextFactory, ILogger<AppManagementRepository> logger, IMapper mapper, IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<IEnumerable<dynamic>?> GetAppsFeatAuth(string? appid, string? app_feature_id, string? comcode, string? app_object)
        {
            try
            {
                await Task.Delay(1);

                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return null;
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20("", "itv_sys.SP_APPS_MANAGER", "Get_Feature_Wise_Comp_Permission", appid ?? "", app_feature_id ?? "", comcode ?? "", app_object ?? "");

                if (ds == null)
                {
                    _logger.LogWarning($"SP 'Get_Feature_Wise_Comp_Permission' returned empty table.");
                    return null;
                }

                var resultDto = ds.Tables[0].DataTableToDynamicList();

                return resultDto;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Failed to get AppsFeatAuth information");
                return null;
            }
        }
    }
}
