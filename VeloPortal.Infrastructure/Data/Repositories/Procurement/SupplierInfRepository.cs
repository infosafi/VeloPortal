using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Procurement
{
    public class SupplierInfRepository : ISupplierInf
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public SupplierInfRepository(
          IDbContextFactory<VeloPortalDbContext> dbContextFactory,
          IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<IEnumerable<DtoVendorSupply>?> GetSupplierSupplyItems(string? comcod)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<IEnumerable<DtoVendorSupply>?>(null);
                }
                IEnumerable<DtoVendorSupply>? lst = null;
                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_scm.SP_PROCUREMENT_MGT", "Get_Supplier_Supply_Items");
                if (ds == null || ds.Tables.Count == 0)
                {
                    return await Task.FromResult<IEnumerable<DtoVendorSupply>?>(null);
                }

                lst = ds.Tables[0].DataTableToList<DtoVendorSupply>();
                return await Task.FromResult<IEnumerable<DtoVendorSupply>?>(lst);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<IEnumerable<DtoVendorSupply>?>(null);
            }
        }

        public async Task<bool> DeleteVendorSuplyById(int supItemId)
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                var item = await dbContext.VendorSuply.FindAsync(supItemId);

                if (item == null) return false;

                dbContext.VendorSuply.Remove(item);
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
