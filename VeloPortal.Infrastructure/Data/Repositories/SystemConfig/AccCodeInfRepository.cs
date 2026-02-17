using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using VeloPortal.Application.Interfaces.SystemConfig;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.SystemConfig
{
    public class AccCodeInfRepository : IAccCodeInf
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<AccCodeInfRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;
        public AccCodeInfRepository(
         IDbContextFactory<VeloPortalDbContext> dbContextFactory,
         ILogger<AccCodeInfRepository> logger,
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

        public async Task<IEnumerable<dynamic>?> GetUserWiseAccCodeBookInfo(string? comcod, string? label, string? search_query, int user_id, bool? is_manual_post, bool? is_last_head)
        {
            try
            {
                await Task.Delay(1);
                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return await Task.FromResult<IEnumerable<dynamic>?>(null);
                }
                IEnumerable<dynamic>? lst = null;
                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_sys.SP_CODEBOOK_MGT", "Get_Acc_Code_Book_Information", label ?? string.Empty,
                   search_query?.ToString() ?? string.Empty, user_id.ToString(), is_manual_post.ToString() ?? "", is_last_head.ToString() ?? "");
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<IEnumerable<dynamic>?>(null);
                }
                lst = ds.Tables[0].DataTableToDynamicList();
                return await Task.FromResult<IEnumerable<dynamic>?>(lst);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Acc Code Book Retrival Failed");

                return await Task.FromResult<IEnumerable<dynamic>?>(null);
            }
        }
    }
}
