using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.DTOs.SystemConfig;
using VeloPortal.Application.Interfaces.SystemConfig;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;

namespace VeloPortal.Infrastructure.Data.Repositories.SystemConfig
{
    public class ComApiInfRepository  : IComApiInf
    {

        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<ComApiInfRepository> _logger;
        private readonly IMapper _mapper;

        public ComApiInfRepository(
            IDbContextFactory<VeloPortalDbContext> dbContextFactory,
            ILogger<ComApiInfRepository> logger,
            IMapper mapper)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DtoComApiInf>?> GetCompanyAllComApiInf(string? comcod, string? gencode)
        {
            try
            {
                await using var dbContext = _dbContextFactory.CreateDbContext();

                var query = dbContext.ComApiInf.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(comcod))
                    query = query.Where(p => p.comcod == comcod);

                if (!string.IsNullOrWhiteSpace(gencode))
                    query = query.Where(p => p.gencode == gencode);

                var comApiInfos = await query.ToListAsync();

                return _mapper.Map<IEnumerable<DtoComApiInf>>(comApiInfos)
                       ?? Enumerable.Empty<DtoComApiInf>();
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Failed to retrieve company API information");
                return Enumerable.Empty<DtoComApiInf>();
            }
        }
    }
}
