using VeloPortal.Application.DTOs.ProjectImplementation;
using VeloPortal.Domain.Entities.ProjectImplementation;

namespace VeloPortal.Application.Interfaces.ProjectImplementation
{
    public interface IContractorProfile
    {
        Task<bool> IsContractorLoginAllowedAsync(string comcod, long contractorProfileId);
        Task<bool> SetEmailVerifiedAsync(string comcod, long contractorProfileId);
        Task<ContractorProfile?> FindContractorByEmailAsync(string comcod, string email);
        Task<ContractorProfile?> InsertOrUpdateContractor(ContractorProfile obj);
        Task<dynamic?> FindUserByContractorIdAsync(string comcod,string? fromdate, string? todate, string contractor_profile_id);
        Task<IEnumerable<DtoContractorDashboardCounter>?> GetContractorDashboardCounter(string? comcod, string? user_role, string? user_id, string? res_code);
        Task<bool> SaveContractorSuply(IEnumerable<ContractorSuply> contractorSuply);

    }
}
