using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class CustomerExperienceController(IPrjStdSelection _prjStdSelection) : ControllerBase
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

                var response = await _prjStdSelection.GetCustomerFinishingItemSelection(comcod!, sale_unit_id);

                if (response == null)
                    return NotFound(ApiResponse<dynamic>.FailureResponse(new List<string> { " customer finishing item selections not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

                return Ok(ApiResponse<dynamic>.SuccessResponse(response, message: $"customer finishing item selections loaded successfully. (Data Found: {response.Count()})"));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailureResponse(new List<string> { $"Error getting customer finishing item selections: {ex.Message}" }, "Internal Server Error"));
            }
        }


        /// <summary>
        /// Save or Update project standard item selections
        /// </summary>
        /// <param name="prjstdselections">This the Post body of project standard item selection.</param> 
        /// <returns>true or false</returns>
        [HttpPost("save-project-standard-item-selections")]
        public async Task<IActionResult> SaveProjectStandarditemslections([FromBody] List<PrjStdSelection>? prjstdselections)
        {
            if (prjstdselections == null || prjstdselections.Count() == 0)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Empty Payload" }, "Payload should not be null or empty."));
            }

            var savedstdselection = await _prjStdSelection.InsertOrUpdateProjStdItemSelections(prjstdselections);

            if (!savedstdselection.IsSuccess)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { savedstdselection.message, "Saved Failed" }, savedstdselection.message));
            }

            return Ok(ApiResponse<dynamic>.SuccessResponse(savedstdselection.IsSuccess, "project standard item selections Saved Successfully"));
        }
        #endregion
    }
}
