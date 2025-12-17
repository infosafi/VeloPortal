using VeloPortal.Application.DTOs.Common;

namespace VeloPortal.Application.Interfaces.Authentication
{
    public interface IPortalAuthUser
    {
        Task<DtoUserInf?> ValidateCredentialsAsync(string comcod, string user_type, string user_or_email, string password);
    }
}
