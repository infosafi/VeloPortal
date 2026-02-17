using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using VeloPortal.Application.Interfaces.ProjectManagement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.ProjectManagement
{
    /// <summary>
    /// API Controller for Project Management operations.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [SwaggerTag("Project Management")]
    public class ProjectController(IUnitInfo _unitInfo) : ControllerBase
    {

        #region UnitInfo
        /// <summary>
        /// Retrieves unit information by company code, account code, resource code, and unit name.
        /// Returns unit details from the stored procedure as a keyed dynamic list (e.g., { Key: "UnitDetails", Data: [...] }).
        /// </summary>
        /// <param name="comcod">The company code (required, e.g., '11001').</param>
        /// <param name="acccode">The account code (required, e.g., '160100010001,160100010002').</param>
        /// <param name="unitType">The resource code filter (optional, e.g., '500100103%'). Defaults to '%'.</param>
        /// <param name="floor">The unit name filter (optional, e.g., '%'). Defaults to '%'.</param>
        /// <returns>A response containing unit details.</returns>
        /// <response code="200">Unit information retrieved successfully.</response>
        /// <response code="400">Invalid parameters provided.</response>
        /// <response code="404">No unit information found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("get-unit-info")]
        [SwaggerOperation(
            Summary = "Get Unit Information by Code",
            Description = "Fetches unit details from the stored procedure itv_prj.[SP_PROJECT_MANAGEMENT] with call type 'Get_Project_Units_Info'."
        )]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<dynamic>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUnitInfo(
            [BindRequired][FromQuery] string comcod,
            [BindRequired][FromQuery] string acccode,
            [FromQuery] string? unitType = "%",
            [FromQuery] string? floor = "%")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comcod) || string.IsNullOrWhiteSpace(acccode))
                {
                    return BadRequest(ApiResponse<object>.FailureResponse(
                        new List<string> { "Company code (comcod) and account code (acccode) are required." },
                        "Invalid Request Parameters"));
                }

                var response = await _unitInfo.GetUnitInfoByCodeAsync(comcod, acccode, unitType, floor);

                if (response == null)
                {
                    return NotFound(ApiResponse<object>.FailureResponse(
                        new List<string> { "Unit information not found." },
                        ErrorTrackingExtension.ErrorMsg ?? "Error Occurred"));
                }

                if (!response.Any())
                {
                    return NotFound(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(response, message: "No Unit Data Found"));
                }

                return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(response, message: "Unit Data Found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse(
                        new List<string> { $"Error retrieving unit information: {ex.Message}" },
                        "Internal Server Error"));
            }
        }
        #endregion UnitInfo
    }
}
