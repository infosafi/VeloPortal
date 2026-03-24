namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoPortalAuthUser
    {
        public string? comcod { get; set; }
        public string? user_or_email { get; set; }
        public string? password { get; set; }
        public string user_type { get; set; } = String.Empty;
    }
}
