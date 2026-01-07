using VeloPortal.Application.DTOs.Common;

namespace VeloPortal.Application.Interfaces.Authentication
{
    public interface IPortalAuthUser
    {
        Task<DtoUserInf?> ValidateCredentialsAsync(string comcod, string user_type, string user_or_email, string password);
        Task<DtoUserInf?> FindUserByEmailOrPhoneAsync(string comcod, string user_type, string user_or_email);
        Task<bool> UpdatePasswordAsync(string comcod, string user_type, string userId, string new_password, string portal_role);
    }
}
