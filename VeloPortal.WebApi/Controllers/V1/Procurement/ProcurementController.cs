using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.Procurement
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProcurementController : ControllerBase
    {
   
        private readonly IPurRFQInf _purRFQInf;

        public ProcurementController( IPurRFQInf purRFQInf)
        {
            _purRFQInf = purRFQInf;
        }

        /// <summary>
        /// Get Single rfq Info.
        /// </summary>
        /// <param name="comcod">The Company Code (e.g., 11001).</param>
        /// <param name="rfqid">rfq Id: ex:1</param>
        /// <returns>Returns an <see cref="IActionResult"/>dynamic Single rfq info</returns>
        [HttpGet("get-single-rfq-info")]
        public async Task<IActionResult> GetSingleRFQInfo(string comcod, string? rfqid)
        {
            if (string.IsNullOrEmpty(rfqid))
            {
                return BadRequest(ApiResponse<dynamic>.FailureResponse(new List<string> { "rfqid is required" }));
            }
            var response = await _purRFQInf.GetSingleRFQList(comcod, rfqid);
            if (response == null)
                return NotFound(ApiResponse<dynamic>.FailureResponse(
                    new List<string> { "Single rfq request not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            if (response.Count() == 0)
                return NotFound(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(response, message: "No Data Found"));


            return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(response, message: response.Count() + " Data Found"));
        }

        /// <summary>
        /// Save/Update RFQ Information
        /// </summary>
        /// <param name="purrfq">This the Post body RFQ Details information</param>     
        /// <returns>rfq no</returns>
        [HttpPost("update-requestforquote-details")]
        public async Task<IActionResult> UpdateRequestForQuoteInformation([FromBody] DtoRFQInf purrfq)
        {
            if (purrfq == null)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(
                new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "RFQ Details Should Not Null or Empty"));
            }

            var response = await _purRFQInf.InsertorUpdateRequestForQuoteInfo(purrfq);

            if (response == null)
                return BadRequest(ApiResponse<string>.FailureResponse(
                   new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "RFQ Update Failed"));

            if (response == "")
                return BadRequest(ApiResponse<string>.FailureResponse(
                   new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "RFQ Update Failed"));


            return Ok(ApiResponse<string>.SuccessResponse(response ?? "", message: "RFQ Update Successfully"));


        }
    }
}
