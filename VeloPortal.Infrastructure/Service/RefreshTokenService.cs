using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.Interfaces.Common;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Infrastructure.Data.DataContext;

namespace VeloPortal.Infrastructure.Service
{
    public class RefreshTokenService(IDbContextFactory<VeloPortalDbContext> _dbContextFactory,
    ILogger<RefreshTokenService> _logger
   ) : IRefreshTokenService
    {



        public async Task<RefreshToken?> GetByTokenAsync(string? token)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    var result = await dbContext.RefreshToken
                 .FirstOrDefaultAsync(rt => rt.token == token);
                    if (result == null)
                    {
                        return null;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Token Not found");
                return null;
            }

        }

        public async Task RevokeAsync(string? token)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var refreshToken = await dbContext.RefreshToken
            .FirstOrDefaultAsync(rt => rt.token == token);

                if (refreshToken != null)
                {
                    refreshToken.is_revoked = true;
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task SaveAsync(RefreshToken? token)
        {
            if (token != null)
            {

                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    await dbContext.RefreshToken.AddAsync(token);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
