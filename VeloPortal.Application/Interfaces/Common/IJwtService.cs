using VeloPortal.Application.DTOs.Authentication;

namespace VeloPortal.Application.Interfaces.Common
{
    public interface IJwtService
    {
        string GenerateAccessToken(DtoUserInf? user);
        string GenerateRefreshToken();
    }
}
