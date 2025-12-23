using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.ServiceRequest
{
    public class DtoPortalUsersServiceRequest
    {
        public string? comcod { get; set; }
        public string service_no { get; set; } = String.Empty;
        public long service_req_id { get; set; }
        public DateTime service_req_date { get; set; }
        public string complain_details { get; set; } = String.Empty;
        public string current_step { get; set; } = String.Empty;
        public bool is_done { get; set; }
        public DateTime est_done_date { get; set; }
    }
}
