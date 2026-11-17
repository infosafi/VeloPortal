using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Entities.ProjectImplementation;

namespace VeloPortal.Application.Interfaces.ProjectImplementation
{
    public interface IContractorProfile
    {
        Task<ContractorProfile> InsertOrUpdateContractor(ContractorProfile obj);
    }
}
