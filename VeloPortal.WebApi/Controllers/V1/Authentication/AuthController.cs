using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.Authentication;
using VeloPortal.Application.DTOs.Common;
using VeloPortal.Application.Interfaces.Authentication;
using VeloPortal.Application.Interfaces.Common;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Enums;
using VeloPortal.Domain.Extensions;
using VeloPortal.WebApi.Helpers;

namespace VeloPortal.WebApi.Controllers.V1.Authentication
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshRepo;
        private readonly IPortalAuthUser _userRepo;
        private readonly IPassRecovery _passRecovery;

        public AuthController(IJwtService jwtService, IRefreshTokenService refreshRepo,
            IPortalAuthUser userRepo, IPassRecovery passRecovery)
        {
            _jwtService = jwtService;
            _refreshRepo = refreshRepo;
            _userRepo = userRepo;
            _passRecovery = passRecovery;
        }


        [HttpPost("portal-login")]
        public async Task<IActionResult> Login(DtoPortalAuthUser dto)
        {
            if (dto.comcod.Length == 0)
            {

                return BadRequest(new { Success = false, message = "Company Selection is missing!" });

            }
            if (dto.user_or_email.Length == 0)
            {
                return BadRequest(new { Success = false, message = "Username or Email Mandatory!" });

            }
            if (dto.password.Length == 0)
            {
                return BadRequest(new { Success = false, message = "Password Mandatory!" });

            }
            string encpassword = EncryptionExtension.PasswordEnc(dto.password);
            var user = await _userRepo.ValidateCredentialsAsync(dto.comcod, dto.user_type, dto.user_or_email, encpassword);
            if (user == null)
            {
                return Unauthorized(new { Success = false, message = "User or Password Invalid", twofactor = false });
            }
            if (user.unq_id == 0)
            {
                return Unauthorized(new { Success = false, message = "User or Password Invalid", twofactor = false });
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _refreshRepo.SaveAsync(new RefreshToken
            {
                token = refreshToken,
                expires = DateTime.UtcNow.AddDays(7),
                user_id = user.unq_id
            });

            return Ok(new
            {
                accessToken,
                refreshToken,
                userinfo = user,
                Success = true,
                message = "Successfully Logged In"
            });
        }


        //[HttpPost("refresh")]
        //public async Task<IActionResult> Refresh(DtoJwtToken dto)
        //{
        //    var storedToken = await _refreshRepo.GetByTokenAsync(dto.RefreshToken);
        //    if (storedToken == null || storedToken.is_revoked || storedToken.expires < DateTime.UtcNow)
        //        return Unauthorized();


        //    var user = await _userRepo.GetUserInfoByIdAsync(Convert.ToInt32(storedToken.user_id));
        //    var newAccessToken = _jwtService.GenerateAccessToken(user);
        //    var newRefreshToken = _jwtService.GenerateRefreshToken();

        //    storedToken.is_revoked = true;
        //    await _refreshRepo.RevokeAsync(dto.RefreshToken);

        //    if (user != null)
        //    {
        //        await _refreshRepo.SaveAsync(new RefreshToken
        //        {
        //            token = newRefreshToken,
        //            expires = DateTime.UtcNow.AddDays(7),
        //            user_id = user.unq_id
        //        });
        //        return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });

        //    }
        //    else
        //    {
        //        return Ok(new { message = "Token not refreshed" });
        //    }


        //}

        /// <summary>
        /// Sends a 6-digit OTP to the user's email for password recovery.
        /// </summary>
        /// <param name="dto">Contains the user's email or phone and the user-agent string.</param>
        /// <returns>
        /// Returns 200 OK if OTP is sent successfully, 
        /// 400 BadRequest for invalid input or inactive user, 
        /// 404 NotFound if the user does not exist or OTP fails to store.
        /// </returns>
        /// <remarks>
        /// The generated OTP is valid for 5 minutes. The request logs IP address and user-agent info for audit purposes. 
        /// If the user does not exist or is inactive, no email is sent.
        /// </remarks>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] DtoUserForgotPassword dto)
        {
            if (string.IsNullOrWhiteSpace(dto.user_or_email))
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "Phone or Email is required."));

            var user = await _userRepo.FindUserByEmailOrPhoneAsync(dto.comcod, dto.user_type, dto.user_or_email);
            if (user == null || user.unq_id == 0)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "User not found." }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            // Generate 6-digit OTP and expiry
            var otp = new Random().Next(100000, 999999).ToString();
            var createdDate = DateTime.UtcNow;
            var expiryDate = createdDate.AddMinutes(5);

            // Get client IP and user-agent
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();


            // Build PassRecovery entity
            var passRecovery = new PassRecovery
            {
                user_id = user.unq_id,
                user_email = user.user_email,
                recovery_otp = otp,
                created_date = createdDate,
                expiry_date = expiryDate,
                execution_date = DateTime.Parse("1900-01-01"),
                request_agent = dto.user_agent,
                ip_address = ipAddress,
                is_recovered = false,
                is_expired = false,
                portal_role = !string.IsNullOrEmpty(user.user_role) ? user.user_role : "51"
            };

            // Save to DB
            var result = await _passRecovery.InsertOrUpdatePassRecovery(passRecovery, HelperEnums.Action.Add.ToString());
            if (!result)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "Failed to store OTP." }, ErrorTrackingExtension.ErrorMsg ?? "Error Occured"));

            // Compose and send OTP email
            var subject = "Your VeloPortal Verification Code";

            var message = $@"
                    <div style=""background-color: #f6f9fc; padding: 40px 0; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;"">
                        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 500px; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1);"">
                            <tr>
                                <td class=""bg-primary"" style=""background-color: #727cf5; padding: 25px; text-align: center;"">
                                    <h1 style=""margin: 0; color: #ffffff; font-size: 24px; font-weight: bold;"">VeloPortal</h1>
                                </td>
                            </tr>
        
                            <tr>
                                <td style=""padding: 40px 30px;"">
                                    <h2 style=""margin: 0 0 20px 0; font-size: 20px; color: #313a46;"">Forgot your password?</h2>
                                    <p style=""margin: 0 0 25px 0; font-size: 16px; color: #6c757d; line-height: 1.6;"">
                                        Hello <strong>{user.fullname}</strong>,<br><br>
                                        We received a request to reset your password. Use the verification code below to proceed. <strong>This code is valid for 5 minutes.</strong>
                                    </p>

                                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                        <tr>
                                            <td align=""center"" style=""background-color: #f1f3fa; border: 1px dashed #727cf5; border-radius: 8px; padding: 25px;"">
                                                <span style=""font-size: 12px; color: #727cf5; text-transform: uppercase; letter-spacing: 2px; font-weight: bold; display: block; margin-bottom: 10px;"">
                                                    Your Verification Code
                                                </span>
                                                <div style=""font-size: 42px; font-weight: 800; color: #313a46; letter-spacing: 10px; font-family: 'Courier New', Courier, monospace;"">
                                                    {otp}
                                                </div>
                                            </td>
                                        </tr>
                                    </table>

                                    <p style=""margin: 25px 0 0 0; font-size: 14px; color: #98a6ad; text-align: center;"">
                                        If you did not request this, you can safely ignore this email. No changes will be made to your account.
                                    </p>
                
                                    <hr style=""border: 0; border-top: 1px solid #eef2f7; margin: 30px 0;"">

                                    <p style=""font-size: 14px; color: #313a46; margin: 0;"">
                                        Best regards,<br>
                                        <strong>VeloPortal Support Team</strong>
                                    </p>
                                </td>
                            </tr>

                            <tr>
                                <td style=""padding: 20px; background-color: #f9f9f9; text-align: center; border-top: 1px solid #eef2f7;"">
                                    <p style=""font-size: 12px; color: #98a6ad; margin: 0;"">
                                        &copy; {DateTime.Now.Year} VeloPortal. All rights reserved. <br>
                                        This is an automated security message. Please do not reply.
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </div>";

            await EmailHelper.SendEmail(user.user_email, subject, message);

            return Ok(ApiResponse<bool>.SuccessResponse(true, message: "OTP sent to registered email address."));
        }

        /// <summary>
        /// Verifies the 6-digit OTP.
        /// </summary>
        /// <param name="dto">Contains the OTP entered by the user.</param>
        /// <returns>
        /// 200 OK if OTP is sent successfully.  
        /// 400 Bad Request for invalid input or inactive user.  
        /// 404 Not Found if the user does not exist or OTP storage fails.
        /// </returns>
        /// <remarks>
        /// Generates a one-time 6-digit OTP valid for 5 minutes.   
        /// No email is sent if the user is inactive or not found.
        /// </remarks>
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] DtoVerifyOtp dto)
        {
            if (string.IsNullOrWhiteSpace(dto.otp))
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "OTP is required." }, "Invalid input."));

            //  Fetch latest OTP record
            var otpRecord = await _passRecovery.GetLatestOtpAsync(dto.otp);
            if (otpRecord == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "Invalid or expired OTP." }, "OTP not found."));

            //  Check OTP state
            if (otpRecord.is_recovered)
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "OTP already used." }, "Invalid OTP."));

            if (otpRecord.is_expired || DateTime.UtcNow > otpRecord.expiry_date)
            {
                otpRecord.is_expired = true;
                await _passRecovery.InsertOrUpdatePassRecovery(otpRecord, HelperEnums.Action.Update.ToString());
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "OTP expired." }, "Expired OTP."));
            }

            //  Compare OTP safely
            if (!string.Equals(otpRecord.recovery_otp?.Trim(), dto.otp.Trim(), StringComparison.Ordinal))
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Invalid OTP." }, "Incorrect OTP."));

            //  Mark OTP as verified and issue reset token
            otpRecord.is_recovered = true;
            otpRecord.execution_date = DateTime.UtcNow;
            otpRecord.expiry_date = DateTime.UtcNow.AddMinutes(5);

            await _passRecovery.InsertOrUpdatePassRecovery(otpRecord, HelperEnums.Action.Update.ToString());

            return Ok(ApiResponse<PassRecovery>.SuccessResponse(otpRecord, "OTP verified successfully. Use this token to reset your password."));

        }

        /// <summary>
        /// Resets the user's password using a verified otp.
        /// </summary>
        /// <param name="dto">
        /// Contains the verified OTP and new password credentials.
        /// </param>
        /// <returns>
        /// 200 OK if the password is reset successfully.  
        /// 400 Bad Request if input is invalid or the otp is expired.  
        /// 404 Not Found if the associated user cannot be found.
        /// </returns>
        /// <remarks>
        /// Final step of the password recovery process.  
        /// Requires a valid otp from <c>verify-otp</c>.  
        /// Token expires in 5 minutes and is invalidated after use.  
        /// Passwords are securely encrypted before storage.
        /// </remarks>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] DtoResetPassword dto)
        {
            //  Input validation
            if (string.IsNullOrWhiteSpace(dto.new_password) ||
                string.IsNullOrWhiteSpace(dto.confirm_password))
            {
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "All fields are required." }, "Invalid input."));
            }

            if (dto.new_password != dto.confirm_password)
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Passwords do not match." }, "Validation error."));

            //  Fetch recovery record
            var recoveryRecord = await _passRecovery.GetLatestOtpAsync(dto.otp);
            if (recoveryRecord == null)
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Invalid or expired token." }, "Invalid token."));

            // Validate token expiry
            if (recoveryRecord?.expiry_date == null || recoveryRecord.expiry_date < DateTime.UtcNow)
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Reset token expired." }, "Expired token."));

            //  Fetch user linked to the OTP record
            var existingUser = await _userRepo.FindUserByEmailOrPhoneAsync("11001", "Customer", recoveryRecord.user_email);
            if (existingUser == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "User not found." }, "Invalid user."));

            string encryptedPassword = EncryptionExtension.PasswordEnc(dto.new_password);

            string userRole = existingUser.user_role ?? "";
            string userPrefix = userRole.StartsWith("15") ? "15" : "51";

            bool isUpdateSuccess = await _userRepo.UpdatePasswordAsync( "11001", "Customer", existingUser.unq_id.ToString(), encryptedPassword, userPrefix );

            if (!isUpdateSuccess)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(
                   new List<string> { "Failed to update password in database." }, "Database Error."));
            }

            recoveryRecord.is_expired = true;
            recoveryRecord.expiry_date = DateTime.UtcNow;
            await _passRecovery.InsertOrUpdatePassRecovery(recoveryRecord, HelperEnums.Action.Update.ToString());

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Password has been reset successfully."));
        }

        /// <summary>
        /// Resends a new One-Time Password (OTP) to the user's registered email.
        /// </summary>
        /// <param name="dto">
        /// Contains the user's email or username to resend the OTP.
        /// </param>
        /// <returns>
        /// 200 OK if the OTP was successfully resent.  
        /// 400 Bad Request for invalid input or inactive user.  
        /// 404 Not Found if the user does not exist.
        /// </returns>
        /// <remarks>
        /// This endpoint regenerates a new 6-digit OTP and sends it to the user's registered email.  
        /// It replaces any previous OTP and is valid for a limited time (e.g., 5 minutes).  
        /// Can be used before calling the <c>verify-otp</c> endpoint.
        /// </remarks>
        [HttpPost("resend-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] DtoResendOtp dto)
        {
            //  Validate input
            if (string.IsNullOrWhiteSpace(dto.user_or_email))
                return BadRequest(ApiResponse<string>.FailureResponse(
                    new List<string> { "Email is required." }, "Invalid input."));

            try
            {
                //  Find user by email
                var user = await _userRepo.FindUserByEmailOrPhoneAsync(dto.comcod, dto.user_type, dto.user_or_email);
                if (user == null)
                    return NotFound(ApiResponse<string>.FailureResponse(
                        new List<string> { "User not found." }, "Invalid email."));

                //  Generate new OTP
                var otp = new Random().Next(100000, 999999).ToString();
                var createdDate = DateTime.UtcNow;
                var expiryDate = createdDate.AddMinutes(5);


                // Get client IP and user-agent
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();


                // Build PassRecovery entity
                var passRecovery = new PassRecovery
                {
                    user_id = user.unq_id,
                    user_email = user.user_email,
                    recovery_otp = otp,
                    created_date = createdDate,
                    expiry_date = expiryDate,
                    execution_date = DateTime.Parse("1900-01-01"),
                    request_agent = dto.user_agent,
                    ip_address = ipAddress,
                    is_recovered = false,
                    is_expired = false,
                    portal_role = !string.IsNullOrEmpty(user.user_role) ? user.user_role : "51"
                };


                var saveResponse = await _passRecovery.InsertOrUpdatePassRecovery(passRecovery, HelperEnums.Action.Add.ToString());
                if (!saveResponse)
                {
                    return BadRequest(ApiResponse<string>.FailureResponse(
                        new List<string> { "Failed to store OTP information." }, "Database operation failed."));
                }

                //  Prepare email message
                var subject = "Password Reset Request";
                var message = $@"
                    <div style=""background-color: #f6f9fc; padding: 40px 0; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;"">
                        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 500px; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1);"">
                            <tr>
                                <td class=""bg-primary"" style=""background-color: #727cf5; padding: 25px; text-align: center;"">
                                    <h1 style=""margin: 0; color: #ffffff; font-size: 24px; font-weight: bold;"">VeloPortal</h1>
                                </td>
                            </tr>
        
                            <tr>
                                <td style=""padding: 40px 30px;"">
                                    <h2 style=""margin: 0 0 20px 0; font-size: 20px; color: #313a46;"">Verify your identity</h2>
                                    <p style=""margin: 0 0 25px 0; font-size: 16px; color: #6c757d; line-height: 1.6;"">
                                        Hello <strong>{user.fullname}</strong>,<br><br>
                                        We received a request to reset your password. Use the verification code below to proceed. <strong>This code is valid for 5 minutes.</strong>
                                    </p>

                                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                        <tr>
                                            <td align=""center"" style=""background-color: #f1f3fa; border: 1px dashed #727cf5; border-radius: 8px; padding: 25px;"">
                                                <span style=""font-size: 12px; color: #727cf5; text-transform: uppercase; letter-spacing: 2px; font-weight: bold; display: block; margin-bottom: 10px;"">
                                                    Your Verification Code
                                                </span>
                                                <div style=""font-size: 42px; font-weight: 800; color: #313a46; letter-spacing: 10px; font-family: 'Courier New', Courier, monospace;"">
                                                    {otp}
                                                </div>
                                            </td>
                                        </tr>
                                    </table>

                                    <p style=""margin: 25px 0 0 0; font-size: 14px; color: #98a6ad; text-align: center;"">
                                        If you did not request this, you can safely ignore this email. No changes will be made to your account.
                                    </p>
                
                                    <hr style=""border: 0; border-top: 1px solid #eef2f7; margin: 30px 0;"">

                                    <p style=""font-size: 14px; color: #313a46; margin: 0;"">
                                        Best regards,<br>
                                        <strong>VeloPortal Support Team</strong>
                                    </p>
                                </td>
                            </tr>

                            <tr>
                                <td style=""padding: 20px; background-color: #f9f9f9; text-align: center; border-top: 1px solid #eef2f7;"">
                                    <p style=""font-size: 12px; color: #98a6ad; margin: 0;"">
                                        &copy; {DateTime.Now.Year} VeloPortal. All rights reserved. <br>
                                        This is an automated security message. Please do not reply.
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </div>";

                //  Send email
                await EmailHelper.SendEmail(user.user_email, subject, message);


                return Ok(ApiResponse<string>.SuccessResponse("A new OTP has been sent to your registered email.", "OTP resent successfully."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.FailureResponse(
                        new List<string> { "An unexpected error occurred while resending OTP." },
                        ErrorTrackingExtension.ErrorMsg ?? ex.Message));
            }
        }


        /// <summary>
        /// For Change Password of User 
        /// </summary>
        /// <param name="dto">User Info</param>         
        /// <returns>Pasword update</returns>
        [HttpPost("Change-Password")]
        [Authorize]
        public async Task<IActionResult> UserChangePassword(DtoUserChangePassword dto)
        {
            if (dto.comcod?.Length == 0)
            {

                return BadRequest(new { Success = false, message = "Company Selection is missing!" });

            }
            if (dto.new_password?.Length == 0)
            {
                return BadRequest(new { Success = false, message = "New Password Mandatory!" });

            }

            var existingUser = await _userRepo.FindUserByEmailOrPhoneAsync(dto.comcod, "Customer", dto.user_email);

            if (existingUser == null)
                return NotFound(ApiResponse<string>.FailureResponse(
                    new List<string> { "User not found." }, "Invalid user."));

            string encryptedPassword = EncryptionExtension.PasswordEnc(dto.new_password);

            string userRole = existingUser.user_role ?? "";
            string userPrefix = userRole.StartsWith("15") ? "15" : "51";

            bool isUpdateSuccess = await _userRepo.UpdatePasswordAsync("11001", "Customer", existingUser.unq_id.ToString(), encryptedPassword, userPrefix);

            if (!isUpdateSuccess)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(
                   new List<string> { "Failed to update password in database." }, "Database Error."));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Password Updated Successfully."));
        }
    }
}
