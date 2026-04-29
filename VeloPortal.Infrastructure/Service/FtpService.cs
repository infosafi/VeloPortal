using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using VeloPortal.Application.Settings;

namespace VeloPortal.Infrastructure.Service
{
    public class FtpConnectionResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public interface IFtpService
    {
        Task<string> UploadAsync(IFormFile file, string path = "");
        Task<string> DownloadAsBase64Async(string fileUrl);
        Task<bool> DeleteAsync(string fileUrl);
        Task<FtpConnectionResult> CheckConnectionWithMessageAsync();
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

            await EnsureDirectoryExistsAsync(path);
            string fileName = Path.GetFileName(file.FileName);
            string directoryUrl = $"{_ftp.host.TrimEnd('/')}/{_ftp.root.Trim('/')}/{path.Trim('/')}".TrimEnd('/');
            string fileUrl = $"{directoryUrl}/{fileName}";

            var request = CreateRequest(fileUrl, WebRequestMethods.Ftp.UploadFile);

            using (var input = file.OpenReadStream())
            using (var ftpStream = await request.GetRequestStreamAsync())
            {
                await input.CopyToAsync(ftpStream);
            }

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
        public async Task<FtpConnectionResult> CheckConnectionWithMessageAsync()
        {
            try
            {
                string directoryUrl = $"{_ftp.host}/{_ftp.root}".TrimEnd('/');

                var request = CreateRequest(directoryUrl, WebRequestMethods.Ftp.PrintWorkingDirectory);
                request.Timeout = 1000;
                request.ReadWriteTimeout = 1000;
                request.KeepAlive = false;

                using var response = (FtpWebResponse)await request.GetResponseAsync();

                return new FtpConnectionResult
                {
                    IsValid = true,
                    Message = "FTP connection successful."
                };
            }
            catch (WebException ex) when (ex.Response is FtpWebResponse ftpResponse)
            {
                return new FtpConnectionResult
                {
                    IsValid = false,
                    Message = $"FTP connection failed: {ftpResponse.StatusDescription}"
                };
            }
            catch (Exception ex)
            {
                return new FtpConnectionResult
                {
                    IsValid = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

        // ================= HELPERS =================

        private async Task EnsureDirectoryExistsAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            string[] segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string currentUrl = $"{_ftp.host.TrimEnd('/')}/{_ftp.root.Trim('/')}";

            foreach (var segment in segments)
            {
                currentUrl = $"{currentUrl}/{segment}";
                if (!CheckDirectoryExists(currentUrl))
                {
                    try
                    {
                        var request = CreateRequest(currentUrl, WebRequestMethods.Ftp.MakeDirectory);
                        using var response = (FtpWebResponse)await request.GetResponseAsync();
                    }
                    catch (WebException ex)
                    {
                        throw new Exception("Something went wrong." + ex.Message);
                    }
                }
            }
        }

        private bool CheckDirectoryExists(string url)
        {
            try
            {
                var request = CreateRequest(url, WebRequestMethods.Ftp.ListDirectory);
                using var response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                if (ex.Response is FtpWebResponse res &&
                   (res.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable ||
                    res.StatusCode == FtpStatusCode.ActionNotTakenFilenameNotAllowed))
                {
                    return false;
                }
                return false;
            }
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