using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.Interfaces.Documentation;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Documentation;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Documentation
{
    public class DocInfDetRepository : IDocInfDet
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<DocInfDetRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;
        public DocInfDetRepository(
            IDbContextFactory<VeloPortalDbContext> dbContextFactory,
            ILogger<DocInfDetRepository> logger,
            IConfiguration configuration
            )
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<bool> UploadDocument(List<DocInfDet> docInfDets)
        {
            try
            {
                if (docInfDets == null || !docInfDets.Any())
                {
                    _logger.LogWarning("Attempted to upload an empty or null document list.");
                    return false;
                }

                var invalidDocs = docInfDets.Where(d => string.IsNullOrEmpty(d.comcod) || string.IsNullOrEmpty(d.doc_url)).ToList();

                if (invalidDocs.Any())
                {
                    _logger.LogError("Document upload failed: {Count} documents are missing mandatory fields (comcod or doc_url).", invalidDocs.Count);
                    return false;
                }

                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    await dbContext.Set<DocInfDet>().AddRangeAsync(docInfDets);
                    var result = await dbContext.SaveChangesAsync();
                    // Return true if all records were successfully saved
                    return result == docInfDets.Count;
                }
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Document upload Failed during database insertion.");
                return false;
            }
        }
    }
}
