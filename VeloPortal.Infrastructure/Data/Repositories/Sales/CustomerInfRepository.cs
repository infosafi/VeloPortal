using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Application.Interfaces.Sales;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Sales
{
    public class CustomerInfRepository : ICustomerInf
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<CustomerInfRepository> _logger;
        private readonly IMapper _mapper;
        private readonly SPProcessAccess _spProcessAccess;

        public CustomerInfRepository(
            IDbContextFactory<VeloPortalDbContext> dbContextFactory,
            ILogger<CustomerInfRepository> logger,
            IMapper mapper,
            IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;

            var connectionString = configuration.GetConnectionString(DefaultSettings.DefaultDbconnection)
                                  ?? throw new InvalidOperationException("Default connection string not found.");

            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<bool> TogggleCustProfilePrivacy(string? comcod, long? custid, bool isPublic)
        {
            if (custid == null || custid == 0)
            {
                throw new Exception("Company id and Customer id is required.");
            }

            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                var existingData = await dbContext.CustomerInf.FirstOrDefaultAsync(a => a.comcod == comcod && a.custid == custid);

                if (existingData != null)
                {
                    // Update existing record
                    existingData.is_public_for_app = isPublic;
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex.InnerException ?? ex);
                _logger.LogError(ex, "Privacy settings toggle failed.");

                return false;
            }
        }


        public async Task<IEnumerable<dynamic>?> GetRelatedProjPubProfile(string? comcod, string? custcode)
        {
            try
            {
                await Task.Delay(1);
                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return null;
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_portal.SP_USER_OPERATION", "Get_Project_Wise_Public_Profile", custcode ?? string.Empty);
                if (ds == null || ds.Tables.Count == 0)
                {
                    return null;
                }

                var result = ds.Tables[0].DataTableToDynamicList();

                return result;

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Related Projects Public Profile retrieval failed.");

                return null;
            }
        }
    }
}
