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
        /// <param name="request">Unified request body — set <c>action</c> (integer) to control the operation.</param>
        /// <response code="200">Operation completed. Check <c>data.affectedIds</c> for the affected record IDs.</response>
        /// <response code="400">Validation failed or a business rule was violated. See <c>errors</c> for details.</response>
        /// <response code="401">Unauthorized – a valid JWT Bearer token is required.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("save-fin-com-req")]
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
                _ => ApiResponse<DtoFinCompReqResponse>
                        .FailureResponse($"Unknown action: {request.Action}")
            };

            return result.Success ? Ok(result) : BadRequest(result);
        }
        /// <summary>
        /// Retrieve Financial Compliance Request details with comprehensive filtering options.
        /// </summary>
        /// <remarks>
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
