using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Application.DTOs.ServiceRequest;

namespace VeloPortal.Application.Interfaces.Complain
{
    public interface IServiceRequest
    {
        
        Task<IEnumerable<DtoServiceRequest>?> GetUserConcernProjects(string? comcod, string user_role, string unq_id);
    }
}
