using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    }
}
