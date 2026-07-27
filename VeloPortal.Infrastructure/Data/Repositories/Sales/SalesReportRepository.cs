using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using VeloPortal.Application.Interfaces.Sales;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Sales
{
    public class SalesReportRepository : ISalesReport
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<SalesReportRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;

        public SalesReportRepository(
            IDbContextFactory<VeloPortalDbContext> dbContextFactory,
            ILogger<SalesReportRepository> logger,
            IMapper mapper,
            IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<IEnumerable<dynamic>?> GetDelayCharge(string? comcod, string? fromdate, string? todate, string? acccode, string? rescode, string? custcode)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return await Task.FromResult<IEnumerable<dynamic>?>(null);
                }

                IEnumerable<dynamic>? lst = null;

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_sale.SP_REPORT_SALES", "Get_Delay_Charge", fromdate ?? "", todate ?? "", acccode ?? "%", rescode ?? "%", custcode ?? "%");
                if (ds == null)
                {
                    return await Task.FromResult<IEnumerable<dynamic>?>(null);
                }

                lst = ds.Tables[0].DataTableToDynamicList();
                return await Task.FromResult<IEnumerable<dynamic>?>(lst);

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Get Delay Charge Retrival Failed");
                return await Task.FromResult<IEnumerable<dynamic>?>(null);
            }
        }

    }
}
