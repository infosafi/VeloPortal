using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Infrastructure.Service;

namespace VeloPortal.WebApi.Controllers.V1.Documentation
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IFtpService _ftpService;

        public FileController(IFtpService ftpService)
        {
            _ftpService = ftpService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var url = await _ftpService.UploadAsync(file, "SERVICEDOC");
            return Ok(new { url });
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download(string fileUrl)
        {
            var base64 = await _ftpService.DownloadAsBase64Async(fileUrl);
            return Ok(base64);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(string fileUrl)
        {
            await _ftpService.DeleteAsync(fileUrl);
            return Ok("Deleted");
        }
    }
}
