using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Entities.ProjectImplementation;

namespace VeloPortal.Application.Interfaces.ProjectImplementation
{
    public interface IContractorProfile
    {
        Task<bool> IsContractorLoginAllowedAsync(string comcod, long contractorProfileId);
        Task<bool> SetEmailVerifiedAsync(string comcod, long contractorProfileId);
        Task<ContractorProfile?> FindContractorByEmailAsync(string comcod, string email);
        Task<ContractorProfile?> InsertOrUpdateContractor(ContractorProfile obj);
    }
}
