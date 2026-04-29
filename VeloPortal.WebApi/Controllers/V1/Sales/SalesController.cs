using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.Interfaces.Sales;
using VeloPortal.Application.Settings;

namespace VeloPortal.WebApi.Controllers.V1.Sales
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SalesController(IMoneyRcptPmnt _moneyReceipt) : ControllerBase
    {

        /// <summary>
        /// Gets detailed payment schedule with actual payments, balance, and transaction info for a specific sold unit.
        /// </summary>
        /// <remarks>
        /// Returns one row per payment transaction, including:
        /// - Scheduled installment info
        /// - Payment details (MR no, cheque, bank, collector, etc.)
        /// - Calculated balance = scheduled amount - total paid for that installment
        /// </remarks>
        /// <param name="comcod">Company code (required)</param>
        /// <param name="acccode">Project sales account code (18xxxxxxxxxx format) (required)</param>
        /// <param name="urescode">Unit resource code (50xxxxxxxxxx format) (required)</param>
        /// <response code="200">Returns list of payment schedule records with balance</response>
        /// <response code="400">Missing required parameters</response>
        /// <response code="404">No data found for this unit</response>
        [HttpGet("unit-payment-schedule-with-balance")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<dynamic>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> GetUnitPaymentScheduleWithBalance(
            [FromQuery] string comcod,
            [FromQuery] string acccode,
            [FromQuery] string urescode)
        {
            if (string.IsNullOrWhiteSpace(comcod))
                return BadRequest(ApiResponse<string>.FailureResponse("Company code (comcod) is required."));

            if (string.IsNullOrWhiteSpace(acccode) || !acccode.StartsWith("18"))
                return BadRequest(ApiResponse<string>.FailureResponse(
                    "Valid sales account code (starting with 18) is required."));

            if (string.IsNullOrWhiteSpace(urescode) || !urescode.StartsWith("50"))
                return BadRequest(ApiResponse<string>.FailureResponse(
                    "Valid unit resource code (starting with 50) is required."));

            var data = await _moneyReceipt.GetUnitPaymentScheduleWithBalanceAsync(
                comcod: comcod.Trim(),
                acccode: acccode.Trim(),
                urescode: urescode.Trim());

            if (data == null || !data.Any())
            {
                return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(
                    data: new List<dynamic>(),
                    message: "No payment schedule or transactions found for this unit."));
            }

            return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(
                data: data,
                message: $"{data.Count()} payment/schedule record(s) retrieved successfully."));
        }
    }
}
