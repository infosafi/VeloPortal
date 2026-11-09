using VeloPortal.Application.DTOs.Authentication;

namespace VeloPortal.Application.Interfaces.Common
{
    public interface IJwtService
    {
        string GenerateAccessToken(DtoUserInf? user, string? userType = null);
        string GenerateRefreshToken();
        /// <summary>Validates signature/issuer/audience of an access token but ignores expiry (for refresh).</summary>
        System.Security.Claims.ClaimsPrincipal? ReadExpiredToken(string? accessToken);
    }
}
