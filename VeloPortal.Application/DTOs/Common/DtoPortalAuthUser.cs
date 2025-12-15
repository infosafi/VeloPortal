using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Common
{
    public class DtoPortalAuthUser
    {
        public int user_id { get; set; }
        public string? comcod { get; set; }
        public string? email_or_username { get; set; }
        public string user_name { get; set; } = string.Empty;
        public string user_email { get; set; } = string.Empty;
        public string? full_name { get; set; }
        public string? password { get; set; }
        public bool is_active { get; set; }
        public bool is_2fa { get; set; }
        public string user_type { get; set; } = String.Empty;
    }
}
