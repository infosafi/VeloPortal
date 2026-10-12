using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VeloPortal.Application.DTOs.FacilityManagement;
using VeloPortal.Application.DTOs.Sales;
using VeloPortal.Application.Interfaces.FacilityManagement;
using VeloPortal.Application.Interfaces.Sales;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.FacilityManagement;
using VeloPortal.Domain.Entities.Sales;
using VeloPortal.Domain.Enums;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.Sales
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SalesController(IMoneyRcptPmnt _moneyReceipt, ICustomerInf _custInf) : ControllerBase
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

        /// <summary>
        /// Gets payment schedule with individual rows per MR payment (not aggregated).
        /// Returns one row per (schedule item × payment transaction) for the Edison-style client ledger.
        /// </summary>
        [HttpGet("unit-payment-schedule-detail")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<dynamic>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> GetUnitPaymentScheduleDetail([FromQuery] string comcod, [FromQuery] string acccode, [FromQuery] string urescode, [FromQuery] string? type = null, [FromQuery] string? gengrp = null)
        {
            if (string.IsNullOrWhiteSpace(comcod))
                return BadRequest(ApiResponse<string>.FailureResponse("Company code (comcod) is required."));

            if (string.IsNullOrWhiteSpace(acccode) || !(acccode.StartsWith("180") || acccode.StartsWith("240")))
                return BadRequest(ApiResponse<string>.FailureResponse("Invalid account code."));

            if (string.IsNullOrWhiteSpace(urescode) || !urescode.StartsWith("50"))
                return BadRequest(ApiResponse<string>.FailureResponse("Valid unit resource code (starting with 50) is required."));

            var data = await _moneyReceipt.GetUnitPaymentScheduleDetailAsync(
                comcod: comcod.Trim(),
                acccode: acccode.Trim(),
                urescode: urescode.Trim(),
                type: string.IsNullOrWhiteSpace(type) ? null : type.Trim(),
                gengrp: string.IsNullOrWhiteSpace(gengrp) ? null : gengrp.Trim());

            if (data == null || !data.Any())
            {
                return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(
                    data: new List<dynamic>(),
                    message: "No payment schedule or transactions found for this unit."));
            }

            return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(
                data: data,
                message: $"{data.Count()} payment/schedule detail record(s) retrieved successfully."));
        }


        /// <summary>
        /// Toggle Customer Profile Privacy.
        /// </summary>
        /// <param name="custid">This parameter is the customer id such as 1.</param>
        /// <returns>A Response message of the operations result.</returns>
        /// <response code="200">Privacy toggled successfully</response>
        /// <response code="400">Failed to toggle the privacy settings.</response>
        [HttpPost("toggle-cust-profile-privacy")]
        public async Task<IActionResult> ToggleCustomerProfilePrivacy([FromBody] DtoToggleCustProfilePrivacy obj)
        {
            if (obj == null)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "Payload is required."));
            }

            try
            {
                var response = await _custInf.TogggleCustProfilePrivacy(obj);

                if (response)
                {
                    var type = (obj.is_public_for_app == true) ? "Public" : "Private";

                    return Ok(ApiResponse<dynamic>.SuccessResponse(response, message: $"Privacy settings turns {type} successful."));
                }

                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "Privacy settings toggle failed."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "Unknown Error Occured."));
            }
        }


        #region Get all Customer Info 
        /// <summary>
        /// Get All Public Profile related to all the Projects of the Current User
        /// </summary>
        /// <param name="comcod">This the Company Code, such like 11001.</param>
        /// <param name="custcode">This paramter is for customer id , such like 510100205052</param> 
        /// <returns>A Details list of public customer info with project information.</returns>
        [HttpGet("get-related-proj-public-profile")]

        public async Task<IActionResult> GetRelatedProjPubProfile(string? comcod, string? custcode)
        {
            try
            {
                if (string.IsNullOrEmpty(comcod) || string.IsNullOrEmpty(custcode))
                {
                    return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { }, "Company code and Customer code is required."));
                }

                var response = await _custInf.GetRelatedProjPubProfile(comcod, custcode);

                if (response == null)
                    return NotFound(ApiResponse<dynamic>.FailureResponse(new List<string> { "Related Project Public Profile Not Found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));


                return Ok(ApiResponse<IEnumerable<dynamic>?>.SuccessResponse(response, message: $"Related Project Public Profile loaded successfully. (Data Found: {response.Count()})"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailureResponse(new List<string> { $"Error getting related project public profile: {ex.Message}" }, "Internal Server Error"));
            }

        }
        #endregion
    }
}
