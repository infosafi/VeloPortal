using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.Interfaces.FacilityManagement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.FacilityManagement;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.FacilityManagement
{
    public class AdvicesInfRepository : IAdvicesInf
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<AdvicesInfRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public AdvicesInfRepository(IDbContextFactory<VeloPortalDbContext> dbContextFactory, ILogger<AdvicesInfRepository> logger, IMapper mapper, IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }


        public async Task<List<AdvicesInf>?> InsertOrUpdateAdvicesInf(List<AdvicesInf> objs)
        {
            if (objs == null || objs.Count == 0)
                return null;

            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                var results = new List<AdvicesInf>();

                foreach (var obj in objs)
                {
                    // Check existing data
                    var existingData = await dbContext.AdvicesInf.FirstOrDefaultAsync(a => a.fms_message_id == obj.fms_message_id);

                    if (existingData != null)
                    {
                        // Update existing record
                        existingData.customer_role = obj.customer_role;
                        existingData.message_subject = obj.message_subject;
                        existingData.message_body = obj.message_body;
                        existingData.message_attached = obj.message_attached;
                        existingData.seen_date = obj.seen_date;
                        existingData.seen_by = obj.seen_by;

                        results.Add(existingData);
                    }
                    else
                    {
                        // Insert new record
                        var newData = new AdvicesInf
                        {
                            comcod = obj.comcod,
                            rescode = obj.rescode,
                            customer_role = obj.customer_role,
                            message_subject = obj.message_subject,
                            message_body = obj.message_body,
                            message_attached = obj.message_attached,
                            created_date = obj.created_date != default ? obj.created_date : DateTime.UtcNow,
                            seen_date = obj.seen_date,
                            seen_by = obj.seen_by
                        };

                        await dbContext.AdvicesInf.AddAsync(newData);

                        results.Add(newData);
                    }
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return results;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex.InnerException ?? ex);
                _logger.LogError(ex, "Advices Info insert/update failed");

                return null;
            }
        }
    }
}
