using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.Vendor;
using VeloPortal.Application.Interfaces.Vendor;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Vendor;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.Vendor
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class VendorController(IVendorSuply _vendorSuply) : ControllerBase
    {
        /// <summary>
        /// Get Supplier Supply Items List based on company and reference parameters.
        /// </summary>
        /// <param name="comcod">The Company Code, such as 11001.</param>
        /// <param name="refid">The Reference/Vendor ID used for filtering supply items.</param> 
        /// <param name="rescode">The Resource Code prefix to filter specific item types.</param>
        /// <returns>A collection of Vendor Supply details including rates, lead times, and experience.</returns>
        [HttpGet("get-supplier-supply-items")]
        public async Task<IActionResult> GetSupplierSupplyItemsList(string? comcod)
        {
            var response = await _vendorSuply.GetSupplierSupplyItems(comcod);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "Supplier Supply Items not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<DtoVendorSuply>>.SuccessResponse(response, message: response.Count() + " Supplier Supply Items Found"));

        }

        /// <summary>
        /// Get Vendor Dashboard counters for quotations, work orders, and supply metrics
        /// </summary>
        /// <param name="comcod">Company code</param>
        /// <param name="user_role">User role identifier (e.g., '14' for Vendor)</param>
        /// <param name="unq_id">Unique user/vendor identifier</param>
        /// <returns>A collection of dashboard metrics including quotation counts and order amounts</returns>

        [HttpGet("get-vendor-dashboard-counter")]
        public async Task<IActionResult> GetVendorDashboardCounter(string? comcod)
        {
            var response = await _vendorSuply.GetVendorDashboardCounter(comcod);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "No Vendor Dashboard Data Found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<DtoVendorDashboardCounter>>.SuccessResponse(response, message: response.Count() + " Vendor Dashboard Counter Data Found"));

        }

        /// <summary>
        /// Save Supply Item.
        /// </summary>
        /// <param name="supplyItems">All Save Supply Item.</param>
        /// <returns><returns>
        [HttpPost("save-supplier-items")]
        public async Task<IActionResult> SaveVendorSupply([FromBody] List<VendorSuply> supplyItems)
        {
            try
            {
                var response = await _vendorSuply.SaveVendorSuply(supplyItems);
                if(response)
                {
                    return Ok(ApiResponse<bool>.SuccessResponse(response, message: "Vendor Supply saved successfully"));
                }
                return BadRequest(ApiResponse<bool>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "Vendor Supply save Failed"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailureResponse(new List<string> { $"Error saving  Service resources: {ex.Message}" }, "Internal Server Error"));
            }
        }

    }
}
