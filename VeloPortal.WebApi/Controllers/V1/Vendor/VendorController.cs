using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.Vendor;
using VeloPortal.Application.Interfaces.Vendor;
using VeloPortal.Application.Settings;
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
    }
}
