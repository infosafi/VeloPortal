using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.Interfaces.SystemConfig;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.SystemConfig;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.SystemConfig
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class SystemDefaultController(IIndustries _industry) : ControllerBase
    {
        /// <summary>
        /// Gets all Industires Information list, searching with  Active status
        /// </summary>
        /// <param name="is_active">this is for Active or Inactive Industries. if Not select show Both</param>        /// 
        /// <returns>List of Industires</returns>
        [HttpGet("get-industry-info")]
        public async Task<IActionResult> GetIndustriesInformationList(bool? is_active = null)
        {
            var response = await _industry.GetCompanyIndustries(is_active);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "No Industry found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<Industries>>.SuccessResponse(response, message: response.Count() + " Industries found"));


        }
    }
}
