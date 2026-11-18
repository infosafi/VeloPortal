using VeloPortal.Application.DTOs.Procurement;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Entities.ProjectImplementation;
using VeloPortal.Application.DTOs.ProjectImplementation;

namespace VeloPortal.Application.Interfaces.ProjectImplementation
{
    public interface IContractorProfile
    {
        Task<bool> IsContractorLoginAllowedAsync(string comcod, long contractorProfileId);
        Task<bool> SetEmailVerifiedAsync(string comcod, long contractorProfileId);
        Task<ContractorProfile?> FindContractorByEmailAsync(string comcod, string email);
        Task<ContractorProfile?> InsertOrUpdateContractor(ContractorProfile obj);
        Task<dynamic?> FindUserByContractorIdAsync(string comcod, string contractor_profile_id);
        Task<IEnumerable<DtoContractorDashboardCounter>?> GetContractorDashboardCounter(string? comcod, string? user_role, string? user_id, string? res_code);


    }
}
