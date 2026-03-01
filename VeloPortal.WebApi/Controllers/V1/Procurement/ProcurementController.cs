using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
