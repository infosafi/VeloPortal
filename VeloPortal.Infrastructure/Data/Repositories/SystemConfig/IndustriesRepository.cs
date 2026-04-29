using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.Interfaces.SystemConfig;
using VeloPortal.Domain.Entities.SystemConfig;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;

namespace VeloPortal.Infrastructure.Data.Repositories.SystemConfig
{
    public class IndustriesRepository(IDbContextFactory<VeloPortalDbContext> _dbContextFactory,
        ILogger<IndustriesRepository> _logger) : IIndustries
    {

        public async Task<IEnumerable<Industries>?> GetCompanyIndustries(bool? is_active)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    var query = dbContext.Industries.AsNoTracking().AsQueryable();


                    if (is_active.HasValue)
                        query = query.Where(p => p.is_active == is_active.Value);

                    return await query.ToListAsync();


                }

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Industries Retrival Failed");
                return null;
            }
        }
    }
}
