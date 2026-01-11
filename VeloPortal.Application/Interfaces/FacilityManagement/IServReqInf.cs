using VeloPortal.Application.DTOs.ServiceRequest;
using VeloPortal.Domain.Entities.FacilityManagement;

namespace VeloPortal.Application.Interfaces.FacilityManagement
{
    public interface IServReqInf
    {
        Task<DtoPublicServiceRequestDetails?> GetSingleServiceRequestDetailsPublicInformation(string? comcod, long? service_req_id);
        Task<bool> InsertFeedbackServReqInf(DtoServiceRequestFeedback obj);
        Task<DtoServiceRequestDetails?> GetSingleServiceRequestDetailsInformation(string? comcod, long? service_req_id);
        Task<IEnumerable<DtoPortalUsersServiceRequest>?> GetPortalUsersServiceRequests(string? comcod, string user_role, string unq_id);
        Task<IEnumerable<DtoServiceRequest>?> GetUserConcernProjects(string? comcod, string user_role, string unq_id);
        Task<ServReqInf?> InsertOrUpdateServReqInf(ServReqInf obj, string? action);
        Task<IEnumerable<DtoUserServiceRequestCounter>?> GetUserServiceReqeustCounter(string? comcod, string user_role, string unq_id);

    }
}
