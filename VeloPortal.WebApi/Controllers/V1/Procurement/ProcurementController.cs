using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Procurement;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.Procurement
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProcurementController : ControllerBase
    {
   
        private readonly IPurRFQInf _purRFQInf;
        private readonly IPurOrderInf _purOrderInf;
        private readonly IVendorProfile _vendorprofile;

        public ProcurementController( IPurRFQInf purRFQInf, IPurOrderInf purOrderInf, IVendorProfile vendorprofile)
        {
            _purRFQInf = purRFQInf;
            _purOrderInf = purOrderInf;
            _vendorprofile = vendorprofile;
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
            var response = await _vendorprofile.GetVendorDashboardCounter(comcod);

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
                var response = await _vendorprofile.SaveVendorSuply(supplyItems);
                if (response)
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


        /// <summary>
        /// Retrieves a periodic list of Requests for Quotations (RFQ) for a specific vendor.
        /// </summary>
        /// <param name="comcod">The company code identifier.</param>
        /// <param name="rescode">The vendor's resource code (rescode) obtained from their profile.</param>
        /// <returns>A collection of RFQ details including status, dates, and identifiers.</returns>

        [HttpGet("get-periodic-rfq-list")]
        public async Task<IActionResult> GetPeriodicRfqlist(string? comcod, string? rescode)
        {
            var response = await _purRFQInf.GetPeriodicRfqlist(comcod, rescode);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "No RFQ Data Found for the given parameters" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<DtoPeriodicRfqlist>>.SuccessResponse(response, message: response.Count() + " RFQ items retrieved successfully."));

        }

        /// <summary>
        /// Get Single rfq Info.
        /// </summary>
        /// <param name="comcod">The Company Code (e.g., 11001).</param>
        /// <param name="rfqid">rfq Id: ex:1</param>
        /// <returns>Returns an <see cref="IActionResult"/>dynamic Single rfq info</returns>
        [HttpGet("get-single-rfq-info")]
        public async Task<IActionResult> GetSingleRFQInfo(string comcod, string? rfqid)
        {
            if (string.IsNullOrEmpty(rfqid))
            {
                return BadRequest(ApiResponse<dynamic>.FailureResponse(new List<string> { "rfqid is required" }));
            }
            var response = await _purRFQInf.GetSingleRFQList(comcod, rfqid);
            if (response == null)
                return NotFound(ApiResponse<dynamic>.FailureResponse(
                    new List<string> { "Single rfq request not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            if (response.Count() == 0)
                return NotFound(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(response, message: "No Data Found"));


            return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(response, message: response.Count() + " Data Found"));
        }

        /// <summary>
        /// Save/Update RFQ Information
        /// </summary>
        /// <param name="purrfq">This the Post body RFQ Details information</param>     
        /// <returns>rfq no</returns>
        [HttpPost("update-requestforquote-details")]
        public async Task<IActionResult> UpdateRequestForQuoteInformation([FromBody] DtoRFQInf purrfq)
        {
            if (purrfq == null)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(
                new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "RFQ Details Should Not Null or Empty"));
            }

            var response = await _purRFQInf.InsertorUpdateRequestForQuoteInfo(purrfq);

            if (response == null)
                return BadRequest(ApiResponse<string>.FailureResponse(
                   new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "RFQ Update Failed"));

            if (response == "")
                return BadRequest(ApiResponse<string>.FailureResponse(
                   new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "RFQ Update Failed"));


            return Ok(ApiResponse<string>.SuccessResponse(response ?? "", message: "RFQ Update Successfully"));


        }

        /// <summary>
        /// Get Purchase Order List Information
        /// </summary>
        /// <param name="comcod">This the Company Code, such like 11001.</param>
        /// <param name="fromdate">This paramter consider fromdate (such as 01-Oct-2025)</param>
        /// <param name="todate">This paramter consider todate (such as 30-Oct-2025)</param>
        /// <param name="supplier">This is lead supplier code. (such as 990100101001)</param>      

        /// <returns>List of Get Order</returns>
        [HttpGet("get-purchase-order-list")]
        public async Task<IActionResult> GetPurchaseOrderListInfo(string? comcod, string? fromdate, string? todate, string? supplier)
        {
            var response = await _purOrderInf.GetPurchaseOrderList(comcod, fromdate, todate, supplier);

            if (response == null)
                return NotFound(ApiResponse<dynamic>.FailureResponse(
                    new List<string> { "Purchase Order List not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            if (response.Count() == 0)
                return NotFound(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(response, message: "No Data Found"));


            return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(response, message: response.Count() + " Data Found"));
        }
    }
}
