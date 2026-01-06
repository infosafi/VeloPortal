using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoUserForgotPassword
    {
        public string comcod { get; set; }
        public string user_type { get; set; } = String.Empty;
        public string user_or_email { get; set; }
        public string user_role { get; set; }
    }
}
