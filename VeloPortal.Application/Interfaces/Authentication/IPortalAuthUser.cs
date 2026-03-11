using VeloPortal.Application.DTOs.Authentication;
using VeloPortal.Application.DTOs.Common;
using VeloPortal.Domain.Entities.Authentication;

namespace VeloPortal.Application.Interfaces.Authentication
{
    public interface IPortalAuthUser
    {
        Task<DtoUserInf?> ValidateCredentialsAsync(string comcod, string user_type, string user_or_email, string password);
        Task<DtoUserInf?> FindUserByEmailOrPhoneAsync(string comcod, string user_type, string user_or_email);
        Task<VendorProfile?> FindUserByVendorEmailAsync(string comcod, string user_type, string vendor_email);
        Task<DtoCustomer?> FindUserByCustomerEmailAsync(string comcod, string user_type, string cust_email);
        Task<VendorProfile> InsertOrUpdateVendor(VendorProfile obj, string action);
        Task<long> InsertOrUpdateCustomer(DtoCustomer obj);
        Task<bool> UpdatePasswordAsync(string comcod, string user_type, string userId, string new_password, string portal_role);
    }
}
