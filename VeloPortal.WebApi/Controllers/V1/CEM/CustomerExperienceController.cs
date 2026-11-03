using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.CEM;
using VeloPortal.Application.Interfaces.CEM;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.CEM;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.CEM
{

    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CustomerExperienceController(IPrjCustSelection _prjCustSelection) : ControllerBase
    {
        #region Project Standard Item Selection

        /// <summary>
        /// Customer Finishiing Item Selection.
        /// </summary>
        /// <remarks>
        /// This API returns customers Finishing Item Selections.
        /// </remarks>
        /// <param name="comcod">
        /// <i>Example:</i> <c>11001</c> company code
        /// </param>
        /// <param name="sale_unit_id">
        /// <i>Example:</i> <c>14</c>  sale unit id (long) .
        /// </param>
        /// <returns>
        /// returns  Project Standard Item Selection list.
        /// </returns>
        [HttpGet("get-customer-finishing-item-selection")]
        public async Task<IActionResult> GetCustomerFinishingItemSelection(string? comcod, long? sale_unit_id)
        {
            try
            {
                List<string> errors = new List<string>();

                // validate fields
                if (string.IsNullOrEmpty(comcod) || sale_unit_id == null || sale_unit_id == 0)
                    errors.Add("comcod and sale_unit_id fields are required");

                if (errors.Any())
                {
                    return BadRequest(ApiResponse<string>.FailureResponse(errors, string.Join(", ", errors)));
                }

                var response = await _prjCustSelection.GetCustomerFinishingItemSelection(comcod!, sale_unit_id);

                if (response == null)
                    return NotFound(ApiResponse<dynamic>.FailureResponse(new List<string> { " customer finishing item selections not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

                return Ok(ApiResponse<DtoCustFinishItemSelection>.SuccessResponse(response, message: $"customer finishing item selections loaded successfully. (Data Found: {response.Items!.Count()})"));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailureResponse(new List<string> { $"Error getting customer finishing item selections: {ex.Message}" }, "Internal Server Error"));
            }
        }


        /// <summary>
        /// Insert or Update project customer selections
        /// </summary>
        /// <param name="obj"> This the Post body of project customer selection where CustItemSelReqInf and PrjCustSelection data is available.</param> 
        /// <returns>true or false</returns>
        [HttpPost("save-project-customer-selections")]
        public async Task<IActionResult> SaveProjectCustomerSlections([FromBody] DtoPrjCustSelection obj)
        {
            if (obj == null || obj.PrjCustSelection == null || obj.CustItemSelReqInf == null || obj.PrjCustSelection.Count() == 0)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Empty Payload" }, ErrorTrackingExtension.ErrorMsg ?? "Payload should not be null or empty."));
            }

            var response = await _prjCustSelection.InsertOrUpdateProjectCustomerSelections(obj);

            if (!response.IsSuccess)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { response.message, "Saved Failed" }, response.message));
            }

            return Ok(ApiResponse<dynamic>.SuccessResponse(response.IsSuccess, "project customer selections and request Saved Successfully"));
        }
        #endregion
    }
}
