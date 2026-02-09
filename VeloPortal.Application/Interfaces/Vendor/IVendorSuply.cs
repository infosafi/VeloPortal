using VeloPortal.Application.DTOs.Vendor;
using VeloPortal.Domain.Entities.Vendor;

namespace VeloPortal.Application.Interfaces.Vendor
{
    public interface IVendorSuply
    {
        Task<IEnumerable<DtoVendorSuply>?> GetSupplierSupplyItems(string? comcod);
        Task<bool> SaveVendorSuply(IEnumerable<VendorSuply> vendorSuply);
    }
}
