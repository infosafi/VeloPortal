using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.Json;
using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Procurement
{
    public class PurRFQInfRepository : IPurRFQInf

    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<PurRFQInfRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public PurRFQInfRepository(
          IDbContextFactory<VeloPortalDbContext> dbContextFactory,
          ILogger<PurRFQInfRepository> logger,
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

        public async Task<string?> InsertorUpdateRequestForQuoteInfo(DtoRFQInf obj)
        {
            try
            {
                await Task.Delay(1);
                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return "";
                }
                if (obj == null)
                {
                    return "";
                }
                if (obj.DtoRFQDetails == null)
                {
                    return "";
                }
                if (obj.DtoRFQItems == null)
                {
                    return "";
                }

                if (obj.DtoRFQBidder == null)
                {
                    return "";
                }

                string json_purrfqdetails = JsonSerializer.Serialize(obj.DtoRFQDetails);
                string json_purrfqitems = JsonSerializer.Serialize(obj.DtoRFQItems);
                string json_purrfqbiider = JsonSerializer.Serialize(obj.DtoRFQBidder);



                DataSet? result = _spProcessAccess.GetTransInfo20(obj.DtoRFQDetails.comcod ?? "", "itv_scm.SP_PROCUREMENT_MGT", "Update_Request_For_Quote_Info", json_purrfqdetails, json_purrfqitems, json_purrfqbiider);
                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                {
                    return "";
                }

                return result.Tables[0].Rows[0]["rfqno"].ToString();



            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "RFQ Save Failed");

                return "";
            }
        }

        public async Task<IEnumerable<dynamic>?> GetSingleRFQList(string? comcod, string? rfqid)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return await Task.FromResult<IEnumerable<dynamic>?>(null);
                }
                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_scm.SP_PROCUREMENT_MGT", "Get_Single_Rfq_Info", rfqid ?? string.Empty);
                if (ds == null || ds.Tables.Count == 0)
                {
                    _logger.LogWarning("No data returned");
                    return await Task.FromResult<IEnumerable<dynamic>?>(null);
                }

                var result = new List<dynamic>();

                if (ds.Tables.Count > 0)
                {
                    result.Add(new { key = "rfqinfodetails", Data = ds.Tables[0].DataTableToDynamicList() });
                }

                if (ds.Tables.Count > 1)
                {
                    result.Add(new { key = "rfqitemsodetails", Data = ds.Tables[1].DataTableToDynamicList() });
                }

                if (ds.Tables.Count > 1)
                {
                    result.Add(new { key = "rfqbidderdetails", Data = ds.Tables[2].DataTableToDynamicList() });
                }
                return await Task.FromResult<IEnumerable<dynamic>?>(result);

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Single RFQ fetch failed");
                return await Task.FromResult<IEnumerable<dynamic>?>(null);
            }
        }

    }
}
