using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoUserChangePassword
    {
        public string? comcod { get; set; }
        public string? user_role { get; set; }
        public string user_email { get; set; }
        public string? old_password { get; set; }
        public string? new_password { get; set; }
    }
}
