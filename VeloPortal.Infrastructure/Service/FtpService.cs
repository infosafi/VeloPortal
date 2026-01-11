using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using VeloPortal.Application.Interfaces.Common;
using VeloPortal.Application.Settings;

namespace VeloPortal.Infrastructure.Service
{

    public interface IFtpService
    {
        Task<string> UploadAsync(IFormFile file, string path = "");
        Task<string> DownloadAsBase64Async(string fileUrl);
        Task<bool> DeleteAsync(string fileUrl);
    }

    public class FtpService : IFtpService
    {
        private readonly FtpSettings _ftp;

        public FtpService(IOptions<FtpSettings> ftpOptions)
        {
            _ftp = ftpOptions.Value;
        }

        #pragma warning disable SYSLIB0014

        // ================= UPLOAD =================
        public async Task<string> UploadAsync(IFormFile file, string path = "")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            string fileName = Path.GetFileName(file.FileName);
            string directoryUrl = $"{_ftp.host}/{_ftp.root}/{path}".TrimEnd('/');
            string fileUrl = $"{directoryUrl}/{fileName}";

            if (!DirectoryExists(directoryUrl))
            {
                CreateDirectory(directoryUrl);
            }

            var request = CreateRequest(fileUrl, WebRequestMethods.Ftp.UploadFile);

            using var input = file.OpenReadStream();
            using var ftpStream = await request.GetRequestStreamAsync();
            await input.CopyToAsync(ftpStream);

            return fileUrl;
        }

        // ================= DOWNLOAD =================
        public async Task<string> DownloadAsBase64Async(string fileUrl)
        {
            var request = CreateRequest(fileUrl, WebRequestMethods.Ftp.DownloadFile);

            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        // ================= DELETE =================
        public async Task<bool> DeleteAsync(string fileUrl)
        {
            var request = CreateRequest(fileUrl, WebRequestMethods.Ftp.DeleteFile);
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            return response.StatusCode == FtpStatusCode.FileActionOK;
        }

        // ================= HELPERS =================
        private bool DirectoryExists(string url)
        {
            try
            {
                var request = CreateRequest(url, WebRequestMethods.Ftp.ListDirectory);
                using var response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex) when (
                ex.Response is FtpWebResponse res &&
                res.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                return false;
            }
        }

        private void CreateDirectory(string url)
        {
            var request = CreateRequest(url, WebRequestMethods.Ftp.MakeDirectory);
            using var response = (FtpWebResponse)request.GetResponse();
        }

        private FtpWebRequest CreateRequest(string url, string method)
        {
            var request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Credentials = new NetworkCredential(_ftp.username, _ftp.password);
            request.UseBinary = true;
            request.UsePassive = true;
            request.KeepAlive = false;
            request.EnableSsl = false;
            return request;
        }

        #pragma warning restore SYSLIB0014

    }
}
