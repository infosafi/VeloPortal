using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Procurement
{
    public class PurOrderInfRepository : IPurOrderInf
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<PurOrderInfRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;

        public PurOrderInfRepository(IDbContextFactory<VeloPortalDbContext> dbContextFactory, ILogger<PurOrderInfRepository> logger, IMapper mapper, IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }
        public async Task<IEnumerable<dynamic>?> GetPurchaseOrderList(string? comcod, string? fromdate, string? todate, string? supplier)
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

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_scm.SP_PROCUREMENT_MGT", "Get_Purchase_Order_List", fromdate ?? "", todate ?? "", supplier ?? "");
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
                _logger.LogError(ex, "Purchase Order List Retrival Failed");
                return await Task.FromResult<IEnumerable<dynamic>?>(null);
            }
        }


        public async Task<DtoPurOrderInfo?> GetPurchaseOrderInfo(string? comcod, string? pur_ord_id, string? orderno)
        {
            try
            {
                await Task.Delay(1);

                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return null;
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_scm.SP_PROCUREMENT_MGT", "Get_Purchase_Order_Info", pur_ord_id ?? "", orderno ?? "");

                if (ds == null || ds.Tables.Count < 4)
                {
                    _logger.LogWarning($"SP 'Get_Purchase_Order_Info' returned fewer than 4 tables for order: {orderno}");
                    return null;
                }

                IEnumerable<dynamic>? purOrderInfo = ds.Tables[0].DataTableToDynamicList();
                IEnumerable<dynamic>? purOrderItems = ds.Tables[1].DataTableToDynamicList();
                IEnumerable<dynamic>? purOrderSchedule = ds.Tables[2].DataTableToDynamicList();
                IEnumerable<dynamic>? purOrderDoc = ds.Tables[3].DataTableToDynamicList();

                var resultDto = new DtoPurOrderInfo
                {
                    PurOrderInfo = purOrderInfo,
                    PurOrderItems = purOrderItems,
                    PurOrderSchedule = purOrderSchedule,
                    PurOrderDoc = purOrderDoc
                };

                return resultDto;

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Purchase Order Information Failed to Retrive");
                return null;
            }
        }

    }
}
