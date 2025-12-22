using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.ServiceRequest;
using VeloPortal.Application.Interfaces.FacilityManagement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.FacilityManagement;
using VeloPortal.Domain.Enums;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.FacilityManagement
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class FmsMgtController : ControllerBase
    {
        private readonly IServReqInf _servReqInf;


        public FmsMgtController(
            IServReqInf servReqInf)
        {
            _servReqInf = servReqInf;
        }

        /// <summary>
        /// Get concern projects for a specific user
        /// </summary>
        /// <param name="comcod">Company code (optional)</param>
        /// <param name="user_role">User role (Admin, User, Engineer, etc.)</param>
        /// <param name="unq_id">Unique user identifier</param>
        /// <returns>List of concern projects</returns>
        /// <response code="200">Concern projects retrieved successfully</response>
        /// <response code="404">No concern project found</response>
        [HttpGet("get-user-concern-projects")]
        public async Task<IActionResult> GetUserConcernProjectsList(string? comcod, string user_role, string unq_id)
        {
            var response = await _servReqInf.GetUserConcernProjects(comcod, user_role, unq_id);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "User Concern Project not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<DtoServiceRequest>>.SuccessResponse(response, message: response.Count() + " User Concern Project Found"));

        }

        /// <summary>
        /// Create or update a service request
        /// </summary>
        /// <param name="servReqInf">Service request information</param>
        /// <returns>Saved or updated service request data</returns>
        /// <response code="200">Service request saved successfully</response>
        /// <response code="400">Failed to save service request</response>
        [HttpPost("save-service-request-info")]
        public async Task<IActionResult> SaveLeadPreference([FromBody] ServReqInf servReqInf)
        {
            if (servReqInf.service_req_id == 0)
            {
                var savedSerReqInf = await _servReqInf.InsertOrUpdateServReqInf(servReqInf, HelperEnums.Action.Add.ToString());

                if (savedSerReqInf == null)
                {
                    return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "Service request info save Failed"));
                }

                return Ok(ApiResponse<dynamic>.SuccessResponse(savedSerReqInf, message: "Service request info saved successfully"));
            }

            var updatedSerReqInf = await _servReqInf.InsertOrUpdateServReqInf(servReqInf, HelperEnums.Action.Update.ToString());

            if (updatedSerReqInf == null)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "Service request info Update Failed"));
            }

            return Ok(ApiResponse<ServReqInf>.SuccessResponse(updatedSerReqInf, message: "Service request info updated successfully"));

        }

    }
}
