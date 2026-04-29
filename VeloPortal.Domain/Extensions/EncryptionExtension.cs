using System.Security.Cryptography;
using System.Text;

namespace VeloPortal.Domain.Extensions
{
    public class EncryptionExtension
    {
        public static string PasswordEnc(string? password)
        {
            var enc = Encoding.GetEncoding(0);

            byte[] buffer = enc.GetBytes(password);
            var sha1 = SHA1.Create();
            var hash = BitConverter.ToString(sha1.ComputeHash(buffer)).Replace("-", "");
            return hash;
        }
    }
}
