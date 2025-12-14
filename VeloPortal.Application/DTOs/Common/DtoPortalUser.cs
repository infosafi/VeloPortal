using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Common
{
    public class DtoPortalUser
    {
        public int user_id { get; set; }
        public string user_name { get; set; } = string.Empty;
        public string user_email { get; set; } = string.Empty;
        public string? full_name { get; set; }
        public string? password { get; set; }
        public string? user_contact { get; set; }
        public bool is_active { get; set; }
        public string user_type { get; set; } = String.Empty;
    }
}
