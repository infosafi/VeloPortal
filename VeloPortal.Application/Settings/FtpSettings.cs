using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.Settings
{
    public class FtpSettings
    {
        public string host { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string root { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string port { get; set; } = string.Empty;
    }
}
