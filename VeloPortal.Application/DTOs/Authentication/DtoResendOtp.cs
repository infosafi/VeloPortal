using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoResendOtp
    {
        public string? email_username { get; set; } = string.Empty;
        public string? terminal_id { get; set; } = String.Empty;
        public string? user_agent { get; set; } = String.Empty;
        public string? location { get; set; } = String.Empty;
        public string? macaddress { get; set; } = String.Empty;
    }
}
