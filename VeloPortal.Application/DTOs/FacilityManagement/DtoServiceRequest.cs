namespace VeloPortal.Application.DTOs.FacilityManagement
{
    public class DtoServiceRequest
    {
        public string? comcod { get; set; }
        public string acccode { get; set; } = string.Empty;
        public string? accdesc { get; set; }
        public string? shortdesc { get; set; }
        public string? unitcode { get; set; }
        public string? unitdesc { get; set; }
        public string? urescode { get; set; }
        public string? custcode { get; set; }
    }
}
