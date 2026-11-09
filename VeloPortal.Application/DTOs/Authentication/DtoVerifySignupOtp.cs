namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoVerifySignupOtp
    {
        public string comcod { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
    }
}
