namespace VeloPortal.Application.Interfaces.Authentication
{
    public enum OtpVerifyResult { Ok, NotFound, Expired, Invalid, TooManyAttempts }

    public interface ISecureOtp
    {
        /// <summary>Creates a vendor signup OTP. Returns the plain code (to e-mail), or null when
        /// a code was issued less than the cooldown ago or saving failed.</summary>
        Task<string?> IssueVendorSignupOtpAsync(string comcod, int vendorProfileId, string? ipAddress);

        Task<OtpVerifyResult> VerifyVendorSignupOtpAsync(string comcod, int vendorProfileId, string? code);
    }
}
