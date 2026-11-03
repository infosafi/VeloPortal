using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.SystemConfig;
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
    public class SystemDefaultController(IComApiInf _comApiInf, IIndustries _industry, IMessagelog _messagelog) : ControllerBase
    {
        /// <summary>
        /// Gets all Company API Information list for a specific company.
        /// </summary>
        /// <param name="comcod">The company code in Portal.</param>
        /// <param name="gencode">The general code in Portal.</param>
        /// <returns>List of Company API Information</returns>
        [HttpGet("get-com-api-info")]
        public async Task<IActionResult> GetComApiInfoList(string? comcod, string? gencode)
        {
            var response = await _comApiInf.GetCompanyAllComApiInf(comcod, gencode);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "Company API Info not found" },
                    ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<DtoComApiInf>>.SuccessResponse(
                response, message: response.Count() + " Company API Info Found"));
        }

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


        /// <summary>
        /// Saves an SMS/message log entry after sending a notification.
        /// </summary>
        [HttpPost("save-message-log")]
        public async Task<IActionResult> SaveMessageLog([FromBody] DtoMessagelog dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Payload is required." }, "Bad Request"));

            if (string.IsNullOrWhiteSpace(dto.comcod) || string.IsNullOrWhiteSpace(dto.receiver))
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "comcod and receiver are required." }, "Bad Request"));

            try
            {
                dto.created_date = DateTime.Now;
                var result = await _messagelog.SaveMessagelogAsync(dto);

                if (!result)
                    return BadRequest(ApiResponse<bool>.FailureResponse(
                        new List<string> { "Failed to save message log." }, "Save Failed"));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Message log saved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailureResponse(
                        new List<string> { $"Unexpected error: {ex.Message}" }, "Internal Server Error"));
            }
        }
    }
}
