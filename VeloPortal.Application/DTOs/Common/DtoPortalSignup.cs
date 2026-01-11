using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Common
{
    public class DtoPortalSignup
    {
        public string comcod { get; set; } = "11001";
        public string? company_name { get; set; }
        public string email { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string user_type { get; set; } = "vendor";
    }
}
