using VeloPortal.Application.DTOs.Vendor;

namespace VeloPortal.Application.Interfaces.Vendor
{
    public interface IVendorSuply
    {
        Task<IEnumerable<DtoVendorSuply>?> GetSupplierSupplyItems(string? comcod);
    }
}
