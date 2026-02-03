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

        #region Dynamic Code Book

        /// <summary>
        /// Gets List of Resources Code Book
        /// </summary>
        /// <param name="comcod">This the Company Code, such like 11001.</param>
        /// <param name="label">This the Resources label. such like 2,4,7,9,12.</param>
        /// <param name="search_query">This the Dyanmic Query Resources code, Example rescode like '%01%'.</param>       
        /// <param name="user_id">This the Current user, if it set code book retrive as user permission if applicable.</param>        
        /// <param name="is_manual_post">This the Boolean Paramter indicate Chatered Accounts code is for Manual Posting?, Default is false.</param>
        /// <param name="is_last_head">This the Boolean Paramter indicate Last Transection or not default all are retrive.</param>

        /// <returns>List of Resources Code Book</returns>
        [HttpGet("get-dynamic-res-code")]
        public async Task<IActionResult> GetResourcesCodeBookInformation(string? comcod, string? label, string? search_query, int user_id, bool? is_manual_post,
             bool is_last_head = true)
        {
            if (label == null || label.Length == 0 || Convert.ToInt32(label) > 12)
            {
                return NotFound(ApiResponse<DtoResCodeInf>.FailureResponse(
                    new List<string> { "Label is Mandatory or Not Appropriate" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));
            }
            if (search_query == null || search_query.Length == 0)
            {
                return NotFound(ApiResponse<DtoResCodeInf>.FailureResponse(
                    new List<string> { "Searching query needed" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));
            }
            var response = await _rescodeInf.GetUserWiseResCodeBookInfo(comcod, label, search_query, user_id, is_manual_post, is_last_head);


            if (response == null)
                return NotFound(ApiResponse<IEnumerable<DtoResCodeInf>>.FailureResponse(
                    new List<string> { "Resources  Code not found or Query is wrong" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            if (response.Count() == 0)
                return NotFound(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(response, message: "No Data Found"));

            return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(response, message: ""));
        }
        #endregion
    }
}
