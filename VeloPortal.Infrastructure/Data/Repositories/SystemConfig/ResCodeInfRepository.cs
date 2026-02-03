using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.DTOs.SystemConfig;
using VeloPortal.Application.Interfaces.SystemConfig;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.SystemConfig;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.SystemConfig
{
    public class ResCodeInfRepository : IResCodeInf
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<ResCodeInfRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public ResCodeInfRepository(
          IDbContextFactory<VeloPortalDbContext> dbContextFactory,
          ILogger<ResCodeInfRepository> logger,
          IMapper mapper,
          IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<IEnumerable<DtoResCodeInf>?> GetRescodeInfListByStatusAndCode(string? comcod, string? rescode, bool? is_active)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    var query = dbContext.ResCodeInf.Where(x => x.comcod == comcod && x.rescode != null && x.rescode.StartsWith(rescode ?? ""));


                    if (is_active.HasValue)
                        query = query.Where(p => p.is_active == is_active.Value);

                    return await query
                   .AsNoTracking()
                   .ProjectTo<DtoResCodeInf>(_mapper.ConfigurationProvider)
                   .ToListAsync();


                }

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Accounts Code Retrival Failed");
                return null;
            }
        }
        public async Task<IEnumerable<ResCodeInf>?> GetRescodeInfBookGroupCode(string? comcod)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    var query = dbContext.ResCodeInf.Where(x => x.comcod == comcod && x.rescode != null && x.rescode.EndsWith(CodeBookExtension.ThreeZero)).AsNoTracking().AsQueryable();

                    return await query.ToListAsync();


                }

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Accounts Code Retrival Failed");
                return null;
            }
        }

    }
}
