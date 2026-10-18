using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VeloPortal.Application.DTOs.CEM;
using VeloPortal.Application.DTOs.Helper;
using VeloPortal.Application.Interfaces.CEM;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.CEM;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.CEM
{
    public class PrjCustSelectionRepository : IPrjCustSelection
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly IDbContextFactory<VeloPortalDbContext> _dbLogContextFactory;
        private readonly ILogger<PrjCustSelectionRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;

        public PrjCustSelectionRepository(IDbContextFactory<VeloPortalDbContext> dbContextFactory, IDbContextFactory<VeloPortalDbContext> dbLogContextFactory, ILogger<PrjCustSelectionRepository> logger, IMapper mapper, IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _dbLogContextFactory = dbLogContextFactory;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }


        public async Task<DtoCustFinishItemSelection?> GetCustomerFinishingItemSelection(string comcod, long? sale_unit_id)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return await Task.FromResult<DtoCustFinishItemSelection?>(null);
                }

                DtoCustFinishItemSelection? lst = new();

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod, "itv_cem.SP_CUSTOMER_EXPERIENCE_MGT", "Get_Customer_Finishing_Item_Selection", sale_unit_id.ToString() ?? "");

                if (ds == null || ds.Tables == null || ds.Tables.Count < 2)
                {
                    return await Task.FromResult<DtoCustFinishItemSelection?>(null);
                }

                lst.Info = ds.Tables[0].DataTableToDynamicList();
                lst.Items = ds.Tables[1].DataTableToDynamicList();

                return await Task.FromResult<DtoCustFinishItemSelection?>(lst);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Customer Finishing Item selection fetch failed");
                return await Task.FromResult<DtoCustFinishItemSelection?>(null);
            }
        }


        public async Task<DtoResponseHelper> InsertOrUpdateProjectCustomerSelections(DtoPrjCustSelection obj)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");

                    throw new Exception("Save failed.");
                }

                string json_custitemselreqInf = JsonSerializer.Serialize(obj.CustItemSelReqInf);
                string json_prjcustselection = JsonSerializer.Serialize(obj.PrjCustSelection);

                DataSet? ds = _spProcessAccess.GetTransInfo20(obj.CustItemSelReqInf.comcod ?? "", "itv_cem.SP_CUSTOMER_EXPERIENCE_MGT", "Update_Customer_Item_Selection_Info", json_custitemselreqInf, json_prjcustselection);
                if (ds == null || ds.Tables.Count == 0)
                {
                    _logger.LogWarning("Failed to save gatepass information.");

                    throw new Exception("Save failed.");
                }

                return new DtoResponseHelper
                {
                    IsSuccess = true,
                    message = "Customer Item selection and Project Customer selection Saved Successfully.",
                    Data = ds.Tables[0].Rows[0]["csrno"].ToString()
                };

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Project Customer selection Insert or Update Failed");
                return new DtoResponseHelper
                {
                    IsSuccess = false,
                    message = "Project Customer selection  Insert or Update Failed"
                };
            }
        }
    }
}
