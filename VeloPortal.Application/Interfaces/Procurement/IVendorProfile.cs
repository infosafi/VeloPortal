using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Entities.Procurement;

namespace VeloPortal.Application.Interfaces.Procurement
{
    public interface IVendorProfile
    {
        Task<bool> SaveVendorSuply(IEnumerable<VendorSuply> vendorSuply);
        Task<IEnumerable<DtoVendorDashboardCounter>?> GetVendorDashboardCounter(string? comcod, string? user_role, string? user_id, string? res_code);
        Task<VendorProfile?> FindUserByVendorEmailAsync(string comcod, string user_type, string vendor_email);
        Task<VendorProfile> InsertOrUpdateVendor(VendorProfile obj, string action);
    }
}
