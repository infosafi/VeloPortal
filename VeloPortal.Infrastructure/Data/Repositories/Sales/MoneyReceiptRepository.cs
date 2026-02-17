using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.Interfaces.Sales;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Sales
{
    public class MoneyReceiptRepository : IMoneyRcptPmnt
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<MoneyReceiptRepository> _logger;
        private readonly IMapper _mapper;
        private readonly SPProcessAccess _spProcessAccess;

        public MoneyReceiptRepository(
            IDbContextFactory<VeloPortalDbContext> dbContextFactory,
            ILogger<MoneyReceiptRepository> logger,
            IMapper mapper,
            IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;

            var connectionString = configuration.GetConnectionString(DefaultSettings.DefaultDbconnection)
                                  ?? throw new InvalidOperationException("Default connection string not found.");

            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<IEnumerable<dynamic>?> GetUnitPaymentScheduleWithBalanceAsync(string comcod, string acccode,string urescode)
        {
            try
            {
                var ds = _spProcessAccess.GetTransInfo20(
                    comCode: comcod,
                    SQLprocName: "[itv_sale].[SP_UNIT_SALES_MGT]", "Get_Unit_Payment_Schedule_With_Balance", acccode, urescode);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return null;

                var data = ds.Tables[0].DataTableToDynamicList();
                return await Task.FromResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to get unit payment schedule with balance for comcod: {Comcod}, unit: {Urescode}",
                    comcod, urescode);

                ErrorTrackingExtension.SetError(ex);
                return null;
            }
        }
    }
}
