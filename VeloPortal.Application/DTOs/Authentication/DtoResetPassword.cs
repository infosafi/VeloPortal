using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoResetPassword
    {
        public string? otp { get; set; }
        public string? new_password { get; set; }
        public string? confirm_password { get; set; }
    }
}
