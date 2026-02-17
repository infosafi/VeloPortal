using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using VeloPortal.Application.Interfaces.ProjectManagement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.ProjectManagement
{
    public class UnitInfoRepository : IUnitInfo
    {
        private readonly ILogger<UnitInfoRepository> _logger;
        private readonly SPProcessAccess _spProcessAccess;   // Non-nullable
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        public UnitInfoRepository(
            ILogger<UnitInfoRepository> logger,
            IConfiguration configuration, IDbContextFactory<VeloPortalDbContext> dbContextFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var connectionString = configuration.GetConnectionString(DefaultSettings.DefaultDbconnection)
                ?? throw new InvalidOperationException("DefaultDbconnection not found in configuration.");

            _spProcessAccess = new SPProcessAccess(connectionString);
            _dbContextFactory = dbContextFactory;
        }

        // -------------------------------------------------------------
        // 1. Get Unit Info – Synchronous SP call
        // -------------------------------------------------------------
        public Task<IEnumerable<dynamic>?> GetUnitInfoByCodeAsync(
            string? comcod, string? acccode, string? unitType = "%", string? floor = "%")
        {
            try
            {
                DataSet? ds = _spProcessAccess.GetTransInfo20(
                    comcod ?? "",
                    "itv_prj.[SP_PROJECT_MANAGEMENT]",
                    "Get_Project_Units_Info",
                    acccode ?? "",
                    unitType ?? "%",
                    floor ?? "%"
                );

                if (ds == null || ds.Tables.Count == 0)
                {
                    _logger.LogWarning(
                        "No data returned for comcod: {Comcod}, acccode: {Acccode}, unitType: {UnitType}, floor: {Floor}",
                        comcod, acccode, unitType, floor);
                    return Task.FromResult<IEnumerable<dynamic>?>(null);
                }
                // Convert DataTable to dynamic list
                var unitList = ds.Tables[0].DataTableToDynamicList();

                // Sort by urescode ascending
                var sortedList = unitList.OrderBy(u => u.rescode).ToList();
                var result = new List<dynamic>
                {
                    new { Key = "UnitDetails", Data = sortedList }
                };

                _logger.LogInformation(
                    "Retrieved {Count} unit(s) for comcod: {Comcod}, acccode: {Acccode}",
                    ds.Tables[0].Rows.Count, comcod, acccode);

                return Task.FromResult<IEnumerable<dynamic>?>(result);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex,
                    "Failed to retrieve unit info for comcod: {Comcod}, acccode: {Acccode}", comcod, acccode);
                return Task.FromResult<IEnumerable<dynamic>?>(null);
            }
        }
    }
}
