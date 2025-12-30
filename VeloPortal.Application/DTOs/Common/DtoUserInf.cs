using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Common
{
    public class DtoUserInf
    {
        public int unq_id { get; set; }
        public string? comcod { get; set; }
        public string fullname { get; set; } = string.Empty;
        public string? username { get; set; }
        public string? user_email { get; set; }
        public string? user_phone { get; set; }
        public string? user_role { get; set; }
        public string? rolename { get; set; }
    }
}
