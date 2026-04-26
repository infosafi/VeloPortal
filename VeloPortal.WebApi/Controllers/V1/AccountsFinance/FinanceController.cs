using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VeloPortal.Application.DTOs.AccountsFinance;
using VeloPortal.Application.Interfaces.AccountsFinance;
using VeloPortal.Application.Settings;

namespace VeloPortal.WebApi.Controllers.V1.AccountsFinance
{
    /// <summary>
    /// Finance module – Financial Compliance Requests (FCR).
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class FinanceController : ControllerBase
    {
        private readonly IFinCompReq _finComReq;

        public FinanceController(IFinCompReq finComReq)
        {
            _finComReq = finComReq;
        }

        #region Financial Compliance Request

        /// <summary>
        /// Save, Update, Approve, or Cancel a Financial Compliance Request.
        /// </summary>
        /// <remarks>
        /// A single endpoint handles all four lifecycle operations.
        /// The **`action`** field (integer) in the request body determines which operation runs.
        ///
        /// ---
        ///
        /// ## Action Values
        ///
        /// | Value | Action  | Description                          |
        /// |-------|---------|--------------------------------------|
        /// | 0     | Save    | Insert one or more new requests      |
        /// | 1     | Update  | Modify an existing request           |
        /// | 2     | Approve | Approve an existing request          |
        /// | 3     | Cancel  | Cancel an existing request           |
        ///
        /// ---
        /// ## fcrno Auto-Generation (Save only)
        ///
        /// `fcrno` is generated automatically — do **not** send it in the request.
        ///
        /// Format: **`FCR{yyyy}{MM}{nnnnn}`** — 14 characters total.
        ///
        /// | Segment  | Example    | Description                        |
        /// |----------|------------|------------------------------------|
        /// | FCR      | FCR        | Fixed prefix                       |
        /// | {yyyy}   | 2026       | 4-digit year from reqDate          |
        /// | {MM}     | 04         | 2-digit month from reqDate         |
        /// | {nnnnn}  | 00001      | 5-digit sequence, resets monthly   |
        ///
        /// Example: `FCR202604` + `00001` → **`FCR2026040000 1`**
        /// ---
        ///
        /// ## Multiple reqTypes in one Save call
        ///
        /// Each entry in `reqTypes` is saved as a **separate row** sharing the same
        /// header fields (comcod, acccode, rescode, custcode, reqDate, etc.).
        /// Each row gets its own auto-generated `fcrno` and `fin_comp_req_id`.
        /// All generated IDs are returned together in `data.affectedIds`.
        ///
        /// ---
        ///
        /// ## Date Field Formats
        ///
        /// All date fields accept any format parseable by .NET `DateTime`.
        /// ISO 8601 is strongly recommended.
        ///
        /// | Format              | Example                    |
        /// |---------------------|----------------------------|
        /// | ISO 8601 ✅          | `"2026-04-23"`             |
        /// | ISO 8601 with time  | `"2026-04-23T00:00:00Z"`   |
        /// | Short date          | `"04/23/2026"`             |
        /// | Long date           | `"23-Apr-2026"`            |
        ///
        /// ---
        ///
        /// ## Business Rules
        ///
        /// - An **approved** request cannot be updated or cancelled.
        /// - A **cancelled** request cannot be updated or approved.
        /// - `reviewNote` is mandatory for **Approve** and **Cancel**.
        /// - `comcod` is mandatory for all actions.
        /// - For **Update**, only the fields you include will be changed; omitted fields retain their current value.
        ///
        /// ---
        ///
        /// ## Sample Payloads
        ///
        /// **Save – two reqTypes in one call (action = 0):**
        /// ```json
        /// {
        ///   "action": 0,
        ///   "FinCompReqId": 0,
        ///   "comcod": "VC001",
        ///   "acccode": "180010010001",
        ///   "rescode": "500100110002",
        ///   "custcode": "510100101054",
        ///   "reqDate": "2026-04-23",
        ///   "reqTypes": ["Reg NOC", "TPA Vetting"],
        ///   "remarks": "Annual compliance batch.",
        ///   "reqSource": "ERP",
        ///   "ReviewNote": "",
        ///   "UserId": 1
        /// }
        /// ```
        ///
        /// **Update – change remarks and reqType (action = 1):**
        /// ```json
        /// {
        ///   "action": 1,
        ///   "comcod": "VC001",
        ///   "finCompReqId": 1001,
        ///   "remarks": "Corrected remarks.",
        ///   "reqTypes": ["Bank Loan NOC"]
        /// }
        /// ```
        ///
        /// **Approve (action = 2):**
        /// ```json
        /// {
        ///   "action": 2,
        ///   "comcod": "VC001",
        ///   "finCompReqId": 1001,
        ///   "reviewNote": "Documents verified. Approved."
        /// }
        /// ```
        ///
        /// **Cancel (action = 3):**
        /// ```json
        /// {
        ///   "action": 3,
        ///   "comcod": "VC001",
        ///   "finCompReqId": 1001,
        ///   "reviewNote": "Duplicate request. Cancelled."
        /// }
        /// ```
        ///
        /// ---
        ///
        /// ## Response Shapes
        ///
        /// **Success (HTTP 200):**
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "2 Financial Compliance Request(s) saved.",
        ///   "data": {
        ///     "affectedIds": [101, 102],
        ///     "detail": "2 request(s) created successfully."
        ///   },
        ///   "errors": null,
        ///   "pagination": null
        /// }
        /// ```
        ///
        /// **Validation / Business Rule Error (HTTP 400):**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Error",
        ///   "data": null,
        ///   "errors": ["review_note is required when approving a request."],
        ///   "pagination": null
        /// }
        /// ```
        ///
        /// **Unauthorized (HTTP 401):**
        /// ```json
        /// {
        ///   "type": "",
        ///   "title": "Unauthorized",
        ///   "status": 401
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">Unified request body — set <c>action</c> (integer) to control the operation.</param>
        /// <response code="200">Operation completed. Check <c>data.affectedIds</c> for the affected record IDs.</response>
        /// <response code="400">Validation failed or a business rule was violated. See <c>errors</c> for details.</response>
        /// <response code="401">Unauthorized – a valid JWT Bearer token is required.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("save-fin-com-req")]
        [ProducesResponseType(typeof(ApiResponse<DtoFinCompReqResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DtoFinCompReqResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<DtoFinCompReqResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<DtoFinCompReqResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveFinComReq([FromBody] DtoFinCompReqRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<DtoFinCompReqResponse>
                    .FailureResponse(errors, "Validation failed."));
            }

            ApiResponse<DtoFinCompReqResponse> result = request.Action switch
            {
                DtoFinCompReqAction.Save => await _finComReq.SaveAsync(request),
                DtoFinCompReqAction.Update => await _finComReq.UpdateAsync(request),
                DtoFinCompReqAction.Approve => await _finComReq.ApproveAsync(request),
                DtoFinCompReqAction.Cancel => await _finComReq.CancelAsync(request),
                _ => ApiResponse<DtoFinCompReqResponse>
                        .FailureResponse($"Unknown action: {request.Action}")
            };

            return result.Success ? Ok(result) : BadRequest(result);
        }
        /// <summary>
        /// Retrieve Financial Compliance Request details with comprehensive filtering options.
        /// </summary>
        /// <remarks>
        /// ---
        /// 
        /// ## Filter Parameters
        /// 
        /// | Parameter | Type | Description | Example |
        /// |-----------|------|-------------|---------|
        /// | `comcod` | string (required) | Company Code | `"VC001"` |
        /// | `finCompReqId` | long (optional) | Exact Financial Compliance Request ID | `1001` |
        /// | `fcrno` | string (optional) | Exact FCR Number (format: FCRYYYYMMNNNNN) | `"FCR20260400001"` |
        /// | `acccode` | string (optional) | Exact Account Code | `"180010010001"` |
        /// | `rescode` | string (optional) | Exact Resource Code | `"500100110002"` |
        /// | `custcode` | string (optional) | Exact Customer Code | `"510100101054"` |
        /// | `reqtype` | string (optional) | Exact Request Type | `"Reg NOC"` |
        /// | `fromDate` | datetime (optional) | Filter requests from this date (inclusive) | `"2026-01-01"` |
        /// | `toDate` | datetime (optional) | Filter requests up to this date (inclusive) | `"2026-12-31"` |
        /// <param name="comcod">Company Code (required)</param>
        /// <param name="finCompReqId">Financial Compliance Request ID (optional)</param>
        /// <param name="fcrno">FCR Number - format: FCRYYYYMMNNNNN (optional)</param>
        /// <param name="acccode">Account Code (optional)</param>
        /// <param name="rescode">Resource Code (optional)</param>
        /// <param name="custcode">Customer Code (optional)</param>
        /// <param name="reqtype">Request Type (optional)</param>
        /// <param name="fromDate">Start date for filtering (optional)</param>
        /// <param name="toDate">End date for filtering (optional)</param>
        /// <response code="200">Success - Returns matching Financial Compliance Requests</response>
        /// <response code="400">Bad Request - Missing required parameter or invalid filter value</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="500">Internal Server Error - Unexpected error occurred</response>
        ///</remarks>
        [HttpGet("get-fin-com-req-details")]
        [ProducesResponseType(typeof(ApiResponse<DtoFinCompReqDetails>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DtoFinCompReqDetails>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFinComReqDetails([FromQuery, Required] string comcod, [FromQuery] long? finCompReqId = null, [FromQuery] string? fcrno = null, [FromQuery] string? acccode = null, [FromQuery] string? rescode = null, [FromQuery] string? custcode = null, [FromQuery] string? reqtype = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            if (string.IsNullOrWhiteSpace(comcod))
                return BadRequest(ApiResponse<DtoFinCompReqDetails>.FailureResponse("comcod is required."));

            var result = await _finComReq.GetFinCompDetailsAsync(comcod, finCompReqId, fcrno, acccode, rescode, custcode, reqtype, fromDate, toDate);

            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}
