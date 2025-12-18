using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.ServiceRequest;
using VeloPortal.Application.Interfaces.Complain;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.Complain
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ServiceRequestController : ControllerBase
    {
        private readonly IServiceRequest _serviceRequest;


        public ServiceRequestController(
            IServiceRequest serviceRequest)
        {
            _serviceRequest = serviceRequest;

        }


        [HttpGet("get-user-concern-projects")]
        public async Task<IActionResult> GetUserConcernProjectsList(string? comcod, string user_role, string unq_id)
        {
            var response = await _serviceRequest.GetUserConcernProjects(comcod, user_role, unq_id);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "User Concern Project not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<DtoServiceRequest>>.SuccessResponse(response, message: response.Count() + " User Concern Project Found"));

        }
    }
}
