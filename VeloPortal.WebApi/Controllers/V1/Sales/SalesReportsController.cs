using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.Interfaces.Sales;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.Sales
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SalesReportsController(ISalesReport _salesReport) : ControllerBase
    {

        #region Delay Charge

        /// <summary>
        /// Retrieves Delay Charge Report.
        /// </summary>
        /// <remarks>
        /// This API returns  Delay Charge Report between specific time.
        /// </remarks>
        /// <param name="comcod">
        /// <i>Example:</i> <c>11001</c> company code
        /// </param>
        /// <param name="fromdate">This paramter consider fromdate (such as 01-Oct-2025)</param>
        /// <param name="todate">This paramter consider todate (such as 30-Oct-2025)</param>
        /// <param name="acccode">
        /// <i>Example:</i> <c>180010010001</c> acccode for project (18).
        /// </param>
        /// <param name="rescode">
        /// <i>Example:</i> <c>500300128001</c> rescode for unit (50).
        /// </param>
        /// <param name="custcode">
        /// <i>Example:</i> <c>510300128001</c> customercode (51).
        /// </param>
        /// <returns>
        /// returns Periodic Sales History.
        /// </returns>
        [HttpGet("get-delay-charge")]

        public async Task<IActionResult> GetDelayCharge(string? comcod, string? fromdate, string? todate, string? acccode, string? rescode, string? custcode)
        {
            try
            {
                List<string> errors = new List<string>();

                // validate fields
                if (string.IsNullOrEmpty(comcod))
                    errors.Add("Company Code is required");

                if (string.IsNullOrEmpty(rescode) || string.IsNullOrEmpty(acccode))
                    errors.Add("Project Code and Unit Code are required");

                if (errors.Any())
                {
                    return BadRequest(ApiResponse<string>.FailureResponse(errors, string.Join(", ", errors)));
                }

                var response = await _salesReport.GetDelayCharge(comcod, fromdate, todate, acccode, rescode, custcode);

                if (response == null)
                    return NotFound(ApiResponse<dynamic>.FailureResponse(new List<string> { " Delay Charge details not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

                return Ok(ApiResponse<dynamic>.SuccessResponse(response, message: $" Delay Charge Details loaded successfully. (Data Found: {response.Count()})"));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailureResponse(new List<string> { $"Error getting Delay Charge: {ex.Message}" }, "Internal Server Error"));
            }
        }

        #endregion
    }
}
