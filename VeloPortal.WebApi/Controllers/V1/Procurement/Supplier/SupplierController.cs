using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.Procurement.Supplier
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SupplierController(ISupplierInf _supplierInf) : ControllerBase
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
            var response = await _supplierInf.GetSupplierSupplyItems(comcod);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "Supplier Supply Items not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<DtoVendorSupply>>.SuccessResponse(response, message: response.Count() + " Supplier Supply Items Found"));

        }


        /// <summary>
        /// Delete a specific Vendor Supply item.
        /// </summary>
        /// <param name="comcod">Company Code</param>
        /// <param name="rescode">The Item/Resource Code to delete</param>
        /// <returns>A boolean indicating success or failure</returns>
        [HttpDelete("delete-supplier-item/{id}")]
        public async Task<IActionResult> DeleteVendorSupply(int id)
        {
            try
            {
                var response = await _supplierInf.DeleteVendorSuplyById(id);

                if (response)
                {
                    return Ok(ApiResponse<bool>.SuccessResponse(response, message: "Item deleted successfully"));
                }

                return NotFound(ApiResponse<bool>.FailureResponse(
                    new List<string> { "Item not found or already deleted" }, "Delete Failed"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailureResponse(new List<string> { ex.Message }, "Internal Error"));
            }
        }
    }
}
