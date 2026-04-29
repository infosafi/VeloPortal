namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoUserInf
    {
        public int unq_id { get; set; }
        public string? comcod { get; set; }
        public string? rescode { get; set; }
        public string fullname { get; set; } = string.Empty;
        public string? username { get; set; }
        public string? user_email { get; set; }
        public string? user_phone { get; set; }
        public string? user_role { get; set; }
        public string? rolename { get; set; }
    }
}
