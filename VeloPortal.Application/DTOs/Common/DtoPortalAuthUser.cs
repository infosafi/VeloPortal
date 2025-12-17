using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Common
{
    public class DtoPortalAuthUser
    {
        public string comcod { get; set; }
        public string user_or_email { get; set; }
        public string password { get; set; }
        public string user_type { get; set; } = String.Empty;
    }
}
