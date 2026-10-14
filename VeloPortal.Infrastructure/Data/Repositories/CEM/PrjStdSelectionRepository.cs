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
using VeloPortal.Application.DTOs.Helper;
using VeloPortal.Application.Interfaces.CEM;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.CEM;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.CEM
{
    public class PrjStdSelectionRepository : IPrjStdSelection
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly IDbContextFactory<VeloPortalDbContext> _dbLogContextFactory;
        private readonly ILogger<PrjStdSelectionRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;

        public PrjStdSelectionRepository(IDbContextFactory<VeloPortalDbContext> dbContextFactory, IDbContextFactory<VeloPortalDbContext> dbLogContextFactory, ILogger<PrjStdSelectionRepository> logger, IMapper mapper, IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _dbLogContextFactory = dbLogContextFactory;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }


        public async Task<IEnumerable<dynamic>?> GetCustomerFinishingItemSelection(string comcod, long? sale_unit_id)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return await Task.FromResult<IEnumerable<dynamic>?>(null);
                }

                IEnumerable<dynamic>? lst = null;

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod, "itv_cem.SP_CUSTOMER_EXPERIENCE_MGT", "Get_Customer_Finishing_Item_Selection", sale_unit_id.ToString() ?? "");

                if (ds == null)
                {
                    return await Task.FromResult<IEnumerable<dynamic>?>(null);
                }

                lst = ds.Tables[0].DataTableToDynamicList();
                return await Task.FromResult<IEnumerable<dynamic>?>(lst);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Customer Finishing Item selection fetch failed");
                return await Task.FromResult<IEnumerable<dynamic>?>(null);
            }
        }


        public async Task<DtoResponseHelper> InsertOrUpdateProjStdItemSelections(List<PrjStdSelection> PrjStdSelection)
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                foreach (var prjStdSelection in PrjStdSelection)
                {
                    if (prjStdSelection.proj_items_sel_id == 0)
                    {
                        //Add
                        dbContext.PrjStdSelection.Add(prjStdSelection);
                    }
                    else
                    {
                        // Checking if exist or not
                        var exist = await dbContext.PrjStdSelection.FirstOrDefaultAsync(r => r.proj_items_sel_id == prjStdSelection.proj_items_sel_id);

                        if (exist == null)
                        {
                            return new DtoResponseHelper
                            {
                                IsSuccess = false,
                                message = "The PrjStd item selection you are trying to update is not exists."
                            };
                        }

                        dbContext.Entry(exist).CurrentValues.SetValues(prjStdSelection);
                    }
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new DtoResponseHelper
                {
                    IsSuccess = true,
                    message = "Project Standard Item selection Saved Successfully."
                };

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Project Standard Item selection Insert or Update Failed");
                return new DtoResponseHelper
                {
                    IsSuccess = false,
                    message = "Project Standard Item selection  Insert or Update Failed"
                };
            }
        }
    }
}
