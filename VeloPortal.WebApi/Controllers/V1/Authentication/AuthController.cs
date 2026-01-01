using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.DTOs.Common;
using VeloPortal.Application.Interfaces.Authentication;
using VeloPortal.Application.Interfaces.Common;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Extensions;

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


        public AuthController(IJwtService jwtService, IRefreshTokenService refreshRepo,
            IPortalAuthUser userRepo)
        {
            _jwtService = jwtService;
            _refreshRepo = refreshRepo;
            _userRepo = userRepo;

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

        
    }
}
