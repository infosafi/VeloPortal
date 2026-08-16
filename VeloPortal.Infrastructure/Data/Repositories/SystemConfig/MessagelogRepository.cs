using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.DTOs.SystemConfig;
using VeloPortal.Application.Interfaces.SystemConfig;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.SystemConfig;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.SystemConfig
{
    public class MessagelogRepository : IMessagelog
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<MessagelogRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;

        public MessagelogRepository(
            IDbContextFactory<VeloPortalDbContext> dbContextFactory,
            ILogger<MessagelogRepository> logger,
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

        public async Task<bool> SaveMessagelogAsync(DtoMessagelog dto)
        {
            try
            {
                await using var db = await _dbContextFactory.CreateDbContextAsync();

                var entity = new Messagelog
                {
                    comcod = dto.comcod,
                    module_id = dto.module_id,
                    message_type = dto.message_type,
                    gateway = dto.gateway,
                    message_body = dto.message_body,
                    receiver_name = dto.receiver_name,
                    receiver = dto.receiver,
                    reference = dto.reference,
                    created_date = dto.created_date,
                    created_by = dto.created_by,
                    is_status = dto.is_status
                };

                await db.Set<Messagelog>().AddAsync(entity);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save message log");
                return false;
            }
        }
    }
}
