namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoPortalAuthUser
    {
        public string? comcod { get; set; } = String.Empty;
        public string? user_or_email { get; set; } = String.Empty;
        public string? password { get; set; } = String.Empty;
        public string? user_type { get; set; } = String.Empty;
        public string? ip_address { get; set; } = String.Empty;
        public string? user_agent { get; set; } = String.Empty;
        public string? location { get; set; } = String.Empty;
        public string? macaddress { get; set; } = String.Empty;
    } 
}
