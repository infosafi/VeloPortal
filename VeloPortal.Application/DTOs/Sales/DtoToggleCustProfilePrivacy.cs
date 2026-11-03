using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Sales
{
    public class DtoToggleCustProfilePrivacy
    {
        public string comcod { get; set; } = string.Empty;
        public long custid { get; set; }
        public bool is_public_for_app { get; set; }
    }
}
