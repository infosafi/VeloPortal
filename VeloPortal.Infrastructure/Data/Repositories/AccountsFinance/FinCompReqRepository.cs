using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.DTOs.AccountsFinance;
using VeloPortal.Application.Interfaces.AccountsFinance;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.AccountsFinance;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.AccountsFinance
{
    public class FinCompReqRepository : IFinCompReq
    {
        private static readonly DateTime SqlSmallDateTimeMin = new(1900, 1, 1);

        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<FinCompReqRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;

        public FinCompReqRepository(
            IDbContextFactory<VeloPortalDbContext> dbContextFactory,
            ILogger<FinCompReqRepository> logger,
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

        // ─────────────────────────────────────────────────────────────────────
        // FCR-NO GENERATOR
        // ─────────────────────────────────────────────────────────────────────

        public async Task<string> GenerateFcrNoAsync(string comcod, DateTime reqDate)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            return await GenerateFcrNoInternalAsync(comcod, reqDate, db);
        }

        private static async Task<string> GenerateFcrNoInternalAsync(
            string comcod, DateTime reqDate, VeloPortalDbContext db)
        {
            string prefix = $"FCR{reqDate.Year}{reqDate.Month:D2}"; // 9 chars

            string? maxFromDb = await db.FinCompReq
                .Where(x => x.comcod == comcod
                         && x.reqdate.Year == reqDate.Year
                         && x.reqdate.Month == reqDate.Month)
                .Select(x => x.fcrno)
                .OrderByDescending(x => x)
                .FirstOrDefaultAsync();

            string? maxFromLocal = db.FinCompReq.Local
                .Where(x => x.comcod == comcod
                         && x.reqdate.Year == reqDate.Year
                         && x.reqdate.Month == reqDate.Month)
                .Select(x => x.fcrno)
                .OrderByDescending(x => x)
                .FirstOrDefault();

            string? maxFcrNo = new[] { maxFromDb, maxFromLocal }
                .Where(s => !string.IsNullOrEmpty(s))
                .OrderByDescending(s => s)
                .FirstOrDefault();

            int nextSeq = 1;
            if (!string.IsNullOrEmpty(maxFcrNo) && maxFcrNo.Length >= 5)
            {
                string seqPart = maxFcrNo[^5..];
                if (int.TryParse(seqPart, out int parsed))
                    nextSeq = parsed + 1;
            }

            return $"{prefix}{nextSeq:D5}"; // 9 + 5 = 14 chars
        }

        // ─────────────────────────────────────────────────────────────────────
        // SAVE
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<DtoFinCompReqResponse>> SaveAsync(DtoFinCompReqRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Comcod))
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse("comcod is mandatory.");

            if (request.ReqTypes == null || request.ReqTypes.Count == 0)
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                    "At least one reqtype must be provided for Save.");

            try
            {
                DateTime reqDate = request.ReqDate ?? DateTime.UtcNow;
                DateTime now = DateTime.UtcNow;
                int userId = request.UserId ?? 0;

                var insertedIds = new List<long>();
                var insertedFcrNo = new List<string>();
                await using var db = await _dbContextFactory.CreateDbContextAsync();

                foreach (string reqType in request.ReqTypes)
                {
                    string fcrno_auto = await GenerateFcrNoInternalAsync(request.Comcod, reqDate, db);

                    var entity = new FinCompReq
                    {
                        comcod = request.Comcod,
                        fcrno = fcrno_auto,
                        acccode = request.Acccode ?? string.Empty,
                        rescode = request.Rescode ?? string.Empty,
                        custcode = request.Custcode ?? string.Empty,
                        reqtype = reqType,
                        reqdate = reqDate,
                        remarks = request.Remarks ?? string.Empty,
                        delivery_before = request.DeliveryBefore ?? SqlSmallDateTimeMin,
                        created_date = now,
                        created_by = userId,
                        is_cancel = false,
                        is_approved = false,
                        review_note = string.Empty,
                        review_date = SqlSmallDateTimeMin,
                        review_by = 0,
                        req_source = request.ReqSource ?? string.Empty
                    };

                    db.FinCompReq.Add(entity);
                    await db.SaveChangesAsync();
                    insertedIds.Add(entity.fin_comp_req_id);
                    insertedFcrNo.Add(entity.fcrno);
                }

                return ApiResponse<DtoFinCompReqResponse>.SuccessResponse(
                    new DtoFinCompReqResponse
                    {
                        AffectedIds = insertedIds,
                        AffectedFcrNo = insertedFcrNo,
                        Detail = $"{insertedIds.Count} request(s) created successfully."
                    },
                    $"{insertedIds.Count} Financial Compliance Request(s) saved.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveAsync for comcod={Comcod}", request.Comcod);
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                    "An unexpected error occurred while saving the request.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // UPDATE
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<DtoFinCompReqResponse>> UpdateAsync(DtoFinCompReqRequest request)
        {
            if (request.FinCompReqId == null)
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                    "fin_comp_req_id is required for Update.");

            try
            {
                await using var db = await _dbContextFactory.CreateDbContextAsync();

                var entity = await db.FinCompReq
                    .FirstOrDefaultAsync(x => x.fin_comp_req_id == request.FinCompReqId);

                if (entity is null)
                    return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                        $"Record with id {request.FinCompReqId} not found.");

                if (entity.is_approved)
                    return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                        "Cannot update an already approved request.");

                if (entity.is_cancel)
                    return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                        "Cannot update a cancelled request.");

                if (request.Acccode is not null) entity.acccode = request.Acccode;
                if (request.Rescode is not null) entity.rescode = request.Rescode;
                if (request.Custcode is not null) entity.custcode = request.Custcode;
                if (request.ReqSource is not null) entity.req_source = request.ReqSource;
                if (request.Remarks is not null) entity.remarks = request.Remarks;
                if (request.ReqDate.HasValue) entity.reqdate = request.ReqDate.Value;
                if (request.DeliveryBefore.HasValue) entity.delivery_before = request.DeliveryBefore ?? SqlSmallDateTimeMin;

                if (request.ReqTypes is { Count: 1 })
                    entity.reqtype = request.ReqTypes[0];

                await db.SaveChangesAsync();

                return ApiResponse<DtoFinCompReqResponse>.SuccessResponse(
                    new DtoFinCompReqResponse
                    {
                        AffectedIds = new List<long> { entity.fin_comp_req_id },
                        AffectedFcrNo = new List<string> { entity.fcrno },
                        Detail = "Request updated successfully."
                    },
                    "Financial Compliance Request updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for id={Id}", request.FinCompReqId);
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                    "An unexpected error occurred while updating the request.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // APPROVE
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<DtoFinCompReqResponse>> ApproveAsync(DtoFinCompReqRequest request)
        {
            if (request.FinCompReqId == null)
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                    "fin_comp_req_id is required for Approve.");

            if (string.IsNullOrWhiteSpace(request.ReviewNote))
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                    "review_note is required when approving a request.");

            try
            {
                await using var db = await _dbContextFactory.CreateDbContextAsync();

                var entity = await db.FinCompReq
                    .FirstOrDefaultAsync(x => x.fin_comp_req_id == request.FinCompReqId);

                if (entity is null)
                    return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                        $"Record with id {request.FinCompReqId} not found.");

                if (entity.is_cancel)
                    return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                        "Cannot approve a cancelled request.");

                if (entity.is_approved)
                    return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                        "Request is already approved.");

                entity.is_approved = true;
                entity.review_note = request.ReviewNote;
                entity.review_date = DateTime.UtcNow;
                entity.review_by = request.UserId ?? 0;

                await db.SaveChangesAsync();

                return ApiResponse<DtoFinCompReqResponse>.SuccessResponse(
                    new DtoFinCompReqResponse
                    {
                        AffectedIds = new List<long> { entity.fin_comp_req_id },
                        AffectedFcrNo = new List<string> { entity.fcrno },
                        Detail = $"Request {entity.fcrno} approved successfully."
                    },
                    "Financial Compliance Request approved.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ApproveAsync for id={Id}", request.FinCompReqId);
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                    "An unexpected error occurred while approving the request.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // CANCEL
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<DtoFinCompReqResponse>> CancelAsync(DtoFinCompReqRequest request)
        {
            if (request.FinCompReqId == null)
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                    "fin_comp_req_id is required for Cancel.");

            if (string.IsNullOrWhiteSpace(request.ReviewNote))
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                    "review_note is required when cancelling a request.");

            try
            {
                await using var db = await _dbContextFactory.CreateDbContextAsync();

                var entity = await db.FinCompReq
                    .FirstOrDefaultAsync(x => x.fin_comp_req_id == request.FinCompReqId);

                if (entity is null)
                    return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                        $"Record with id {request.FinCompReqId} not found.");

                if (entity.is_cancel)
                    return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                        "Request is already cancelled.");

                if (entity.is_approved)
                    return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                        "Cannot cancel an already approved request.");

                entity.is_cancel = true;
                entity.review_note = request.ReviewNote;
                entity.review_date = DateTime.UtcNow;
                entity.review_by = request.UserId ?? 0;

                await db.SaveChangesAsync();

                return ApiResponse<DtoFinCompReqResponse>.SuccessResponse(
                    new DtoFinCompReqResponse
                    {
                        AffectedIds = new List<long> { entity.fin_comp_req_id },
                        AffectedFcrNo = new List<string> { entity.fcrno },
                        Detail = $"Request {entity.fcrno} cancelled successfully."
                    },
                    "Financial Compliance Request cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CancelAsync for id={Id}", request.FinCompReqId);
                return ApiResponse<DtoFinCompReqResponse>.FailureResponse(
                    "An unexpected error occurred while cancelling the request.");
            }
        }
        // ─────────────────────────────────────────────────────────────────────
        // GET FIN COMP DETAILS
        // ─────────────────────────────────────────────────────────────────────
        public async Task<ApiResponse<DtoFinCompReqDetails>> GetFinCompDetailsAsync(string comcod, long? finCompReqId = null, string? fcrno = null, string? acccode = null, string? rescode = null, string? custcode = null, string? reqtype = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (string.IsNullOrWhiteSpace(comcod))
                return ApiResponse<DtoFinCompReqDetails>.FailureResponse("comcod is mandatory.");

            try
            {
                await using var db = await _dbContextFactory.CreateDbContextAsync(); // just for safety

                var ds = _spProcessAccess?.GetTransInfo50(
                    comCode: comcod,
                    SQLprocName: "itv_acc.SP_ACCOUNTS_MGT",
                    CallType: "Get_Fin_Com_Details",

                    // Desc1 to Desc10 (adjust according to your SP)
                    mDesc1: finCompReqId?.ToString() ?? "",
                    mDesc2: fcrno ?? "",
                    mDesc3: acccode ?? "",
                    mDesc4: rescode ?? "",
                    mDesc5: custcode ?? "",
                    mDesc6: reqtype ?? "",
                    mDesc7: fromDate?.ToString("yyyy-MM-dd") ?? "",
                    mDesc8: toDate?.ToString("yyyy-MM-dd") ?? "",
                    mDesc9: "",  // future use
                    mDesc10: ""
                );

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return ApiResponse<DtoFinCompReqDetails>.SuccessResponse(
                        new DtoFinCompReqDetails { Data = Enumerable.Empty<dynamic>() },
                        "No records found.");
                }

                var data = ds.Tables[0].DataTableToDynamicList();

                return ApiResponse<DtoFinCompReqDetails>.SuccessResponse(
                    new DtoFinCompReqDetails { Data = data },
                    $"{data.Count} record(s) retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFinCompDetailsAsync for comcod={Comcod}", comcod);
                return ApiResponse<DtoFinCompReqDetails>.FailureResponse(
                    "An unexpected error occurred while fetching data.");
            }
        }
    }
}
