using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using VeloPortal.Application.Interfaces.SystemConfig;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.SystemConfig;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.SystemConfig
{
    public class SysGenInfRepository : ISysGenInf
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<SysGenInfRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;
        public SysGenInfRepository(
         IDbContextFactory<VeloPortalDbContext> dbContextFactory,
         ILogger<SysGenInfRepository> logger,
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

        public async Task<IEnumerable<SysGenInf>?> GetSysGenInfGroupCode(string? comcod)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    var query = dbContext.SysGenInf.Where(x => x.comcod == comcod && x.gencod != null && x.gencod.EndsWith(CodeBookExtension.ThreeZero)).AsNoTracking().AsQueryable();

                    return await query.OrderBy(x => x.gencod).ThenBy(x => x.seq).ToListAsync();


                }

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "System General Code Retrival Failed");
                return null;
            }
        }

        public async Task<IEnumerable<SysGenInf>?> GetSysGenInfListByStatusAndGenCode(string? comcod, string? gencod, bool? is_active)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    var query = dbContext.SysGenInf.Where(x => x.comcod == comcod && x.gencod != null && x.gencod.StartsWith(gencod ?? "")).AsNoTracking().AsQueryable();


                    if (is_active.HasValue)
                        query = query.Where(p => p.is_active == is_active.Value);

                    return await query.OrderBy(x => x.gencod).ThenBy(x => x.seq).ToListAsync();


                }

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "System General Code Retrival Failed");
                return null;
            }
        }
    }
}
