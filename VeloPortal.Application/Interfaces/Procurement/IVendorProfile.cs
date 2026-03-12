using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Domain.Entities.Procurement;

namespace VeloPortal.Application.Interfaces.Procurement
{
    public interface IVendorProfile
    {
        Task<bool> SaveVendorSuply(IEnumerable<VendorSuply> vendorSuply);
        Task<IEnumerable<DtoVendorDashboardCounter>?> GetVendorDashboardCounter(string? comcod);
    }
}
