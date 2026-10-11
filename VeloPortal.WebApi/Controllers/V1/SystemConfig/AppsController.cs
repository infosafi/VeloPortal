using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.Interfaces.SystemConfig;
using VeloPortal.Application.Settings;

namespace VeloPortal.WebApi.Controllers.V1.SystemConfig
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]

    public class AppsController(IAppManagement _appManagement) : Controller
    {

        #region Apps Feature Authorization

        /// <summary>
        /// Gets App Feature Authorization information
        /// </summary>
        /// <param name="appid">this is App ID such like 1</param>  
        /// <param name="app_feature_id">this is App Feature ID such like 1</param>  
        /// <param name="comcode">this is company code such like 11001</param>  
        /// <param name="app_object">this is App Object such like 51/93/95/98/99</param>  
        /// <returns>List of  App Feature Authorization data</returns>
        [HttpGet("get-apps-feature-auth")]
        public async Task<IActionResult> GetAppsFeatAuth(string? appid, string? app_feature_id, string? comcode, string? app_object)
        {
            if (string.IsNullOrWhiteSpace(appid))
            {
                return BadRequest(ApiResponse<string>.FailureResponse("App ID is Required", message: "App ID is null or empty"));
            }
            var response = await _appManagement.GetAppsFeatAuth(appid, app_feature_id, comcode, app_object);
            if (response == null)
            {
                return NotFound(ApiResponse<string>.FailureResponse("Not Found!", message: "App Feature Authorization information not found"));
            }
            return Ok(ApiResponse<IEnumerable<dynamic>?>.SuccessResponse(response, message: "App Feature Authorization information found"));
        }

        #endregion

    }
}
