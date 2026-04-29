using VeloPortal.Domain.Entities.Authentication;

namespace VeloPortal.Application.Interfaces.Common
{
    public interface IRefreshTokenService
    {
        Task SaveAsync(RefreshToken? token);
        Task<RefreshToken?> GetByTokenAsync(string? token);
        Task RevokeAsync(string? token);
    }
}
