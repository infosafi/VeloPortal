using VeloPortal.Application.DTOs.ServiceRequest;
using VeloPortal.Domain.Entities.FacilityManagement;

namespace VeloPortal.Application.Interfaces.FacilityManagement
{
    public interface IServReqInf
    {
        Task<IEnumerable<DtoPortalUsersServiceRequest>?> GetPortalUsersServiceRequests(string? comcod, string user_role, string unq_id);
        Task<IEnumerable<DtoServiceRequest>?> GetUserConcernProjects(string? comcod, string user_role, string unq_id);
        Task<ServReqInf?> InsertOrUpdateServReqInf(ServReqInf obj, string? action);
    }
}
