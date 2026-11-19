using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Application.DTOs.ProjectImplementation;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Interfaces.ProjectImplementation;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Procurement;
using VeloPortal.Domain.Entities.ProjectImplementation;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.ProjectImplementation
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ContractorController : ControllerBase
    {
        private readonly IContractorProfile _contractorProfile;
        public ContractorController(IContractorProfile contractorProfile)
        {
            _contractorProfile = contractorProfile;
        }

        /// <summary>
        /// Retrieves the full profile of a contractor by email.
        /// </summary>
        [HttpGet("get-contractor-profile")]
        public async Task<IActionResult> GetContractorProfile(string comcod, string? fromdate, string? todate, string? contractor_profile_id)
        {
            if (string.IsNullOrEmpty(contractor_profile_id))
            {
                return BadRequest(ApiResponse<dynamic>.FailureResponse(new List<string> { "Contractor profile id is required" }));
            }


            var user = await _contractorProfile.FindUserByContractorIdAsync(comcod, fromdate ?? "", todate ?? "", contractor_profile_id ?? "");
            if (user == null)
                return NotFound(new { Success = false, message = "Profile not found." });

            return Ok(new { Success = true, data = user });
        }


        /// <summary>
        /// Updates the contractor profile details.
        /// </summary>
        [HttpPost("update-contractor-profile")]
        public async Task<IActionResult> UpdateContractorProfile([FromBody] DtoContractorProfileUpdate dto)
        {
            // Only a contractor may edit a contractor profile, and only their OWN one: the profile id and company
            // come from the token, never from the request. This endpoint can no longer create vendors
            // (signup does that) and can no longer touch verification / approval / hold / rescode / e-mail.
            if (User.FindFirst("user_type")?.Value != "contractor")
                return Forbid();

            var comcod = User.FindFirst("comcod")?.Value ?? "";
            if (!long.TryParse(User.FindFirst("sub")?.Value, out var contractorId) || contractorId <= 0 || comcod == "")
                return Unauthorized();

            if (dto == null)
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { "Payload is required." }, "Invalid input."));

            // free-text length caps
            if ((dto.company_name?.Length ?? 0) > 200 || (dto.contact_no?.Length ?? 0) > 20 ||
                (dto.compan_overview?.Length ?? 0) > 4000 || (dto.terms_condition?.Length ?? 0) > 4000 ||
                (dto.address?.Length ?? 0) > 500)
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { "A text field is too long." }, "Invalid input."));

            var entity = new ContractorProfile
            {
                contractor_profile_id = contractorId,
                comcod = comcod,
                company_name = dto.company_name ?? "",
                address = dto.address ?? "",
                compan_overview = dto.compan_overview ?? "",
                company_bin = dto.company_bin ?? "",
                contact_no = dto.contact_no ?? "",
                license_no = dto.license_no ?? "",
                num_of_client = dto.num_of_client ?? 0,
                ong_num_of_client = dto.ong_num_of_client ?? 0,
                contact_person = dto.contact_person ?? "",
                secondary_contact_no = dto.secondary_contact_no ?? "",
                designation = dto.designation ?? "",
                is_available = dto.is_available ?? false,
                experience = dto.experience ?? 0,
                terms_condition = dto.terms_condition ?? "",
                business_type = dto.business_type ?? 0,
                payment_mode = dto.payment_mode ?? "",
                owner_name = dto.owner_name ?? "",
                owner_id_no = dto.owner_id_no ?? "",
                owner_tin_no = dto.owner_tin_no ?? "",
                bankcode = dto.bankcode ?? "",
                branch = dto.branch ?? "",
                acc_name = dto.acc_name ?? "",
                acc_number = dto.acc_number ?? "",
                routeno = dto.routeno ?? "",
                links = dto.links ?? "",
                user_photo = dto.user_photo ?? ""
            };

            var updatedResult = await _contractorProfile.InsertOrUpdateContractor(entity);

            if (updatedResult == null)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "Failed to update profile."));
            }

            // Never return the stored entity (it contains the password hash and internal flags).
            return Ok(ApiResponse<bool>.SuccessResponse(true, message: "Profile updated successfully"));
        }

        /// <summary>
        /// Get Contractor Dashboard counters for quotations, work orders
        /// </summary>
        /// <param name="comcod">Company code</param>
        /// <param name="user_role">User role identifier (e.g., '14' for Contractor)</param>
        /// <param name="unq_id">Unique user/contractor identifier</param>
        /// <returns>A collection of dashboard metrics including quotation counts and order amounts</returns>

        [HttpGet("get-contractor-dashboard-counter")]
        public async Task<IActionResult> GetContractorDashboardCounter(string? comcod, string? user_role, string? user_id, string? res_code)
        {
            var response = await _contractorProfile.GetContractorDashboardCounter(comcod, user_role, user_id, res_code);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "No Contractor Dashboard Data Found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<DtoContractorDashboardCounter>>.SuccessResponse(response, message: response.Count() + " Contractor Dashboard Counter Data Found"));

        }

        /// <summary>
        /// Save Supply Item.
        /// </summary>
        /// <param name="supplyItems">All Save Supply Item.</param>
        /// <returns><returns>
        [HttpPost("save-supplier-items")]
        public async Task<IActionResult> SaveVendorSupply([FromBody] List<ContractorSuply> supplyItems)
        {
            try
            {
                var response = await _contractorProfile.SaveContractorSuply(supplyItems);
                if (response)
                {
                    return Ok(ApiResponse<bool>.SuccessResponse(response, message: "Contractor Supply saved successfully"));
                }
                return BadRequest(ApiResponse<bool>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "Contractor Supply save Failed"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<string>.FailureResponse(new List<string> { $"Error saving  Service resources: {ex.Message}" }, "Internal Server Error"));
            }
        }

        /// <summary>
        /// Get Contractor Supply Items List based on company and reference parameters.
        /// </summary>
        /// <param name="comcod">The Company Code, such as 11001.</param>
        /// <param name="contractor_profile_id">The contractor profile unique id like 27.</param>
        /// <param name="refid">The Reference/Vendor ID used for filtering supply items.</param> 
        /// <param name="rescode">The Resource Code prefix to filter specific item types.</param>
        /// <returns>A collection of Contractor Supply details including rates, lead times, and experience.</returns>
        [HttpGet("get-contractor-supply-items")]
        public async Task<IActionResult> GetContractorSupplyItemsList(string? comcod, long? contractor_profile_id)
        {
            var response = await _contractorProfile.GetContractorSupplyItems(comcod, contractor_profile_id);

            if (response == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "Contractor Supply Items not found" }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            return Ok(ApiResponse<IEnumerable<DtoContractorSupply>>.SuccessResponse(response, message: response.Count() + " Contractor Supply Items Found"));

        }
        
        /// <summary>
        /// Delete a specific Contractor Supply item.
        /// </summary>
        /// <param name="comcod">Company Code</param>
        /// <param name="con_item_id">The Item/Resource  Code to delete</param>
        /// <returns>A boolean indicating success or failure</returns>
        [HttpPost("delete-contractor-supply-item")]
        public async Task<IActionResult> DeleteContractorSuplyById(string comcod, long con_item_id)
        {
            try
            {
                var response = await _contractorProfile.DeleteContractorSuplyById(comcod, con_item_id);

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
