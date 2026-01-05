using System.Net;
using System.Net.Mail;
using System.Text;
using VeloPortal.Application.Settings;

namespace VeloPortal.WebApi.Helpers
{
    public class EmailHelper
    {
        private static string? _mailResponse;

        public static async Task SendEmail(string? email, string subject, string message, List<IFormFile>? attachment = null)
        {
            try
            {
                if (email == null || email.Length == 0)
                {
                    _mailResponse = "Receiver Email is missing";
                    await Task.CompletedTask;
                }
                using (var client = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = EmailSettings.FromEmail,
                        Password = EmailSettings.Password
                    };

                    client.UseDefaultCredentials = false;
                    client.Credentials = credential;
                    client.Host = EmailSettings.Host;
                    client.Port = EmailSettings.Port;
                    client.EnableSsl = true;

                    using (var emailMessage = new MailMessage())
                    {
                        emailMessage.IsBodyHtml = true;
                        emailMessage.To.Add(new MailAddress(email ?? ""));
                        emailMessage.From = new MailAddress(EmailSettings.FromEmail);
                        emailMessage.Subject = subject;
                        emailMessage.Body = message;

                        if (attachment != null)
                        {
                            for (int i = 0; i < attachment.Count; i++)
                            {
                                var fileName = Path.GetFileName(attachment[i].FileName);
                                if (attachment[i].Length > 0)
                                {
                                    emailMessage.Attachments.Add(new Attachment(attachment[i].OpenReadStream(), fileName));
                                }
                            }
                        }

                        await client.SendMailAsync(emailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or rethrow
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw;
            }
        }

        public static async Task<string> SendEmailAsync(List<string> ToEmailName, string Subject, string bodyHtml, List<string>? attachments = null)
        {
            _mailResponse = string.Empty;

            using (SmtpClient smtpClient = new SmtpClient(EmailSettings.Host, EmailSettings.Port))
            {

                smtpClient.Credentials = new NetworkCredential(EmailSettings.FromEmail, EmailSettings.Password);
                smtpClient.EnableSsl = true;


                MailMessage message = new MailMessage
                {
                    From = new MailAddress(EmailSettings.FromEmail, EmailSettings.DisplayName),
                    Subject = Subject,
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8,
                    HeadersEncoding = Encoding.UTF8,
                    IsBodyHtml = true,
                    Body = bodyHtml,
                    Priority = MailPriority.High
                };
                foreach (string EmailName in ToEmailName)
                {
                    message.To.Add(new MailAddress(EmailName));
                }
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        message.Attachments.Add(new Attachment(attachment));
                    }
                }

                await smtpClient.SendMailAsync(message);

                _mailResponse = "Email sent successfull";
            }




            return _mailResponse;
        }
    }
}
