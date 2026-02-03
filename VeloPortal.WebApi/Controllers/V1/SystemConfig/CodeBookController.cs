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
    public class CodeBookController(IResCodeInf _rescodeInf) : ControllerBase
    {
        #region Resource Code Book
        /// <summary>
        /// Gets List of Resources Code Book
        /// </summary>
        /// <param name="comcod">This the Company Code, such like 11001.</param>
        /// <param name="rescode">This the Resource code, like 010100101001.</param>
        /// <param name="is_active">This the current status of Resource code.</param>      
        /// <param name="groupcodeonly">This the Boolean Paramter indicate Group code or all code, Default is false.</param>        
        /// 
        /// <returns>List of Resources Code Book</returns>
        [HttpGet("get-res-code-book")]
        public async Task<IActionResult> GetResourseCodeBookInformation(string? comcod, string? rescode, bool? is_active, bool groupcodeonly = false)
        {
            if (groupcodeonly == true)
            {
                var responsegroup = await _rescodeInf.GetRescodeInfBookGroupCode(comcod);

                if (responsegroup == null)
                    return NotFound(ApiResponse<ResCodeInf>.FailureResponse(
                        new List<string> { "Resourses  Code not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

                return Ok(ApiResponse<IEnumerable<ResCodeInf>>.SuccessResponse(responsegroup, message: ""));

            }
            var response = await _rescodeInf.GetRescodeInfListByStatusAndCode(comcod, rescode, is_active);


            if (response == null)
                return NotFound(ApiResponse<IEnumerable<DtoResCodeInf>>.FailureResponse(
                    new List<string> { "Resourses  Code not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            if (response.Count() == 0)
                return NotFound(ApiResponse<IEnumerable<DtoResCodeInf>>.SuccessResponse(response, message: "No Data Found"));


            return Ok(ApiResponse<IEnumerable<DtoResCodeInf>>.SuccessResponse(response, message: ""));
        }

        #endregion
    }
}
