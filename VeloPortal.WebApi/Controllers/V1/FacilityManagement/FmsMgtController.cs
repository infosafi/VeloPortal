using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.ServiceRequest;
using VeloPortal.Application.Interfaces.Complain;
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
        /// Get Single service details Info
        /// </summary>
        /// <param name="comcod">This the Company Code, such like 11001.</param>
        /// <param name="service_req_id">This paramter is for service request id , such like 1,2,3</param> 
        /// <returns>Signle service Details Infomation</returns>
        [HttpGet("get-single-service-details")]

        public async Task<IActionResult> GetSingleServiceDetails(string? comcod, long? service_req_id)
        {
            try
            {
                if (service_req_id == null || service_req_id == 0)
                {
                    return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { }, "service_req_id Should not Empty or null"));
                }
                var response = await _servReqInf.GetSingleServiceRequestDetailsInformation(comcod, service_req_id);

                if (response == null)
                    return NotFound(ApiResponse<dynamic>.FailureResponse(new List<string> { "Single service details info not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

                int serviceCount = response.ServiceInfo?.Count() ?? 0;

                if (serviceCount == 0)
                    return NotFound(ApiResponse<DtoServiceRequestDetails>.SuccessResponse(response, message: "No Single service details found for the given ID."));


                return Ok(ApiResponse<DtoServiceRequestDetails>.SuccessResponse(response, message: $"Single service details loaded successfully. (Data Found: {serviceCount})"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailureResponse(new List<string> { $"Error saving Service resources: {ex.Message}" }, "Internal Server Error"));
            }

        }

        /// <summary>
        /// Retrieves the list of service requests for a specific user.
        /// </summary>
        /// <param name="comcod">
        /// Optional. The company code to filter the results. Can be null.
        /// </param>
        /// <param name="user_role">
        /// Required. The role of the user (e.g., Admin, User, Engineer).
        /// </param>
        /// <param name="unq_id">
        /// Required. The unique identifier of the user.
        /// </param>
        /// <returns>
        /// Returns an <see cref="ApiResponse"/> containing a list of <see cref="DtoPortalUsersServiceRequest"/> objects
        /// if records are found. If no records exist, returns a 404 Not Found response with an error message.
        /// </returns>
        /// <response code="200">
        /// Service requests retrieved successfully. The response contains the list of requests and a message indicating
        /// the number of requests found.
        /// </response>
        /// <response code="404">
        /// No service requests found for the specified user. The response contains an error message.
        /// </response>
        /// <response code="500">
        /// Internal server error if an unexpected error occurs during retrieval.
        /// </response>
        [HttpGet("get-portal-users-service-requests")]
        public async Task<IActionResult> GetPortalUsersServiceRequestsList(string? comcod, string user_role, string unq_id)
        {
            var response = await _servReqInf.GetPortalUsersServiceRequests(comcod, user_role, unq_id);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "User service requests not found" },
                    ErrorTrackingExtension.ErrorMsg ?? "An unexpected error occurred"));

            return Ok(ApiResponse<IEnumerable<DtoPortalUsersServiceRequest>>.SuccessResponse(response, message: response.Count() + " user service request(s) found"));
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

        /// <summary>
        /// Get User wise Service Request Counter
        /// </summary>
        /// <param name="comcod">Company code (optional)</param>
        /// <param name="user_role">User role (Admin, User, Engineer, etc.)</param>
        /// <param name="unq_id">Unique user identifier</param>
        /// <returns>Request Counter</returns>

        [HttpGet("get-user-service-request-counter")]
        public async Task<IActionResult> GetUserServiceRequestCounter(string? comcod, string user_role, string unq_id)
        {
            var response = await _servReqInf.GetUserServiceReqeustCounter(comcod, user_role, unq_id);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "User Service Request Counter Found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<DtoUserServiceRequestCounter>>.SuccessResponse(response, message: response.Count() + " User Service Request Counter Found"));

        }

    }
}
