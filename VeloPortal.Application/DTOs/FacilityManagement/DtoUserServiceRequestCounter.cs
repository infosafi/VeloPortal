namespace VeloPortal.Application.DTOs.FacilityManagement
{
    public class DtoUserServiceRequestCounter
    {
        public string? comcod { get; set; }
        public int requestcount { get; set; } 
        public int ttlobstacle { get; set; }
        public int donereq { get; set; }
        public int pendingreq { get; set; }
        public int pending_feedback { get; set; }
        public int completed { get; set; }
    }
}
