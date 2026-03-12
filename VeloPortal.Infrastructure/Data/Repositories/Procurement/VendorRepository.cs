using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Procurement;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Procurement
{
    public class VendorRepository : IVendorProfile
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public VendorRepository(
          IDbContextFactory<VeloPortalDbContext> dbContextFactory,
          IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<IEnumerable<DtoVendorDashboardCounter>?> GetVendorDashboardCounter(string? comcod)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<IEnumerable<DtoVendorDashboardCounter>?>(null);
                }
                IEnumerable<DtoVendorDashboardCounter>? lst = null;
                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_portal.SP_USER_OPERATION", "Get_Portal_Users_Vendor_Dashboard_Counter");
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<IEnumerable<DtoVendorDashboardCounter>?>(null);
                }

                lst = ds.Tables[0].DataTableToList<DtoVendorDashboardCounter>();
                return await Task.FromResult<IEnumerable<DtoVendorDashboardCounter>?>(lst);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<IEnumerable<DtoVendorDashboardCounter>?>(null);
            }
        }





        public async Task<bool> SaveVendorSuply(IEnumerable<VendorSuply> supplyItems)
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                foreach (var item in supplyItems)
                {
                    var existing = await dbContext.VendorSuply
                        .FirstOrDefaultAsync(x => x.sup_item_id == item.sup_item_id && x.sup_item_id != 0);

                    if (existing == null)
                    {
                        await dbContext.VendorSuply.AddAsync(item);
                    }
                    else
                    {
                        dbContext.Entry(existing).CurrentValues.SetValues(item);
                    }
                }

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return false;
            }
        }
    }
}
