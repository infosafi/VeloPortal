using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.ServiceRequest
{
    public class DtoServiceRequestFeedback
    {
        public long service_req_id { get; set; }
        public string comcod { get; set; } = String.Empty;
        public string service_no { get; set; } = String.Empty;
        public string customer_feedback { get; set; } = String.Empty;
        public int satisfaction { get; set; }
    }
}
