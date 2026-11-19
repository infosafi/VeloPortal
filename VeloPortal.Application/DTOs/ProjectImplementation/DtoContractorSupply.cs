using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.ProjectImplementation
{
    public class DtoContractorSupply
    {
        public long con_item_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string rescode { get; set; } = string.Empty;
        public string resdesc { get; set; } = string.Empty;
        public string spcfcode { get; set; } = string.Empty;
        public string spec_desc { get; set; } = string.Empty;
        public string refid { get; set; } = string.Empty;
        public decimal rate { get; set; }
        public string currency_id { get; set; } = string.Empty;
        public decimal currency_rate { get; set; }
        public int num_of_client { get; set; }
        public int num_of_experience { get; set; }
        public DateTime created_date { get; set; } = new DateTime(1900, 1, 1);
        public string remarks { get; set; } = string.Empty;
        public bool is_audit { get; set; }
        public DateTime audit_date { get; set; } = new DateTime(1900, 1, 1);
        public string audit_by { get; set; } = string.Empty;
        public string audit_by_name { get; set; } = string.Empty;
        public string medium { get; set; } = string.Empty;
        public string lead_time { get; set; } = string.Empty;
    }
}
