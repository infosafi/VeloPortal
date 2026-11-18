using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Application.DTOs.ProjectImplementation;
using VeloPortal.Application.Interfaces.Procurement;
using VeloPortal.Application.Interfaces.ProjectImplementation;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Entities.ProjectImplementation;
using VeloPortal.Domain.Enums;
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
        public async Task<IActionResult> GetContractorProfile(string comcod, string? contractor_profile_id)
        {
            if (string.IsNullOrEmpty(contractor_profile_id))
            {
                return BadRequest(ApiResponse<dynamic>.FailureResponse(new List<string> { "Contractor profile id is required" }));
            }


            var user = await _contractorProfile.FindUserByContractorIdAsync(comcod, contractor_profile_id);
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
    }
}
