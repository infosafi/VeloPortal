using VeloPortal.Application.DTOs.Vendor;
using VeloPortal.Domain.Entities.Vendor;

namespace VeloPortal.Application.Interfaces.Vendor
{
    public interface IVendorSuply
    {
        Task<IEnumerable<DtoVendorSuply>?> GetSupplierSupplyItems(string? comcod);
        Task<bool> DeleteVendorSuplyById(int supItemId);
        Task<bool> SaveVendorSuply(IEnumerable<VendorSuply> vendorSuply);
        Task<IEnumerable<DtoVendorDashboardCounter>?> GetVendorDashboardCounter(string? comcod);
        Task<IEnumerable<DtoPeriodicRfqlist>?> GetPeriodicRfqlist(string? comcod, string? rescode);
    }
}
