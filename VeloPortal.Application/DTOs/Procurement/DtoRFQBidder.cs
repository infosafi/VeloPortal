using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Procurement
{
    public class DtoRFQBidder
    {
        public long pur_rfq_provider_id { get; set; }
        public long pur_rfq_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string rescode { get; set; } = string.Empty;
        public string payment_mode { get; set; } = string.Empty;
        public string supcode { get; set; } = string.Empty;
        public string terms_conditions { get; set; } = string.Empty;
        public bool is_submit { get; set; }
        public decimal advance_amt { get; set; }
        public decimal carrying_amt { get; set; }
        public decimal loading_amt { get; set; }
        public decimal unloading_amt { get; set; }
        public int credit_day { get; set; }
    }
}
