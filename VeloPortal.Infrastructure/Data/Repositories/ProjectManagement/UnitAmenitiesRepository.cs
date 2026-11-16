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
    public class UnitAmenitiesRepository : IUnitAmenities
    {
        private readonly ILogger<UnitAmenitiesRepository> _logger;
        private readonly SPProcessAccess _spProcessAccess;
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;

        private const string SP = "itv_prj.[SP_PROJECT_MANAGEMENT]";
        public UnitAmenitiesRepository(
                ILogger<UnitAmenitiesRepository> logger,
                IConfiguration configuration,
                IDbContextFactory<VeloPortalDbContext> dbContextFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var connectionString = configuration.GetConnectionString(DefaultSettings.DefaultDbconnection)
                ?? throw new InvalidOperationException("DefaultDbconnection not found in configuration.");

            _spProcessAccess = new SPProcessAccess(connectionString);
            _dbContextFactory = dbContextFactory;
        }
        public Task<IEnumerable<dynamic>> GetUnitAmenitiesAsync(string comcod, long unitid = 0, string acccode = "", string rescode = "")
        {
            return Task.Run<IEnumerable<dynamic>>(() =>
            {
                DataSet? ds = _spProcessAccess.GetTransInfo20(
                    comcod, SP, "Get_Unit_Amenities",
                    unitid.ToString(),  // @Desc1 = unitid (0 = all)
                    acccode,            // @Desc2 = acccode ('' = all)
                    rescode             // @Desc3 = rescode ('' = all)
                );

                if (ds == null || ds.Tables.Count == 0)
                    return Enumerable.Empty<dynamic>();

                return ds.Tables[0].DataTableToDynamicList();
            });
        }
    }
}

