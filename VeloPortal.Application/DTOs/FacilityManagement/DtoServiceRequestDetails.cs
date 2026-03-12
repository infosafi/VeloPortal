namespace VeloPortal.Application.DTOs.FacilityManagement
{
    public class DtoServiceRequestDetails
    {
        public IEnumerable<dynamic>? ServiceInfo { get; set; }
        public IEnumerable<dynamic>? ServiceProblemInfo { get; set; }
        public IEnumerable<dynamic>? ServiceResourceInfo { get; set; }
        public IEnumerable<dynamic>? ServiceTimelineInfo { get; set; }
        public IEnumerable<dynamic>? serviceExecution { get; set; }
    }
}
