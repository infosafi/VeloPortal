namespace VeloPortal.Application.DTOs.Authentication
{
    /// <summary>Request body for resending the vendor signup e-mail code (no OTP needed).</summary>
    public class DtoResendSignupOtp
    {
        public string comcod { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}
