using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoResetPassword
    {
        public string? comcod { get; set; }
        public string? user_type { get; set; }
        public string? user_or_email { get; set; }
        public string? otp { get; set; }
        public string? new_password { get; set; }
        public string? confirm_password { get; set; }
    }
}
