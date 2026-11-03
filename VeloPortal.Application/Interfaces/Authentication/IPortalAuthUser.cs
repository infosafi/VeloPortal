using VeloPortal.Application.DTOs.Authentication;
using VeloPortal.Domain.Entities.SystemConfig;

namespace VeloPortal.Application.Interfaces.Authentication
{
    public interface IPortalAuthUser
    {
        Task<DtoUserInf?> ValidateCredentialsAsync(string comcod, string user_type, string user_or_email, string password);
        Task<DtoUserInf?> GetUserInfoByIdRole(string comcod, string user_type, string user_id, string user_role);
        Task<IEnumerable<CompanyInf>?> GetCompanyInfoListByStatus(bool? is_active);
        Task<DtoUserInf?> FindUserByEmailOrPhoneAsync(string comcod, string user_type, string user_or_email);
        Task<DtoCustomer?> FindUserByCustomerEmailAsync(string comcod, string user_type, string cust_email);
        Task<long> InsertOrUpdateCustomer(DtoCustomer obj);
        Task<bool> UpdatePasswordAsync(string comcod, string user_type, string userId, string new_password, string portal_role);
    }
}
