using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.ServiceRequest
{
    public class DtoUserServiceRequestCounter
    {
        public string? comcod { get; set; }
        public int requestcount { get; set; } 
        public int ttlobstacle { get; set; }
        public int donereq { get; set; }
        public int pendingreq { get; set; }
    }
}
