using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Entities.Vendor
{
    [Table("VendorSuply", Schema = "itv_scm")]
    [PrimaryKey(nameof(sup_item_id))]
    public class VendorSuply
    {
        public long sup_item_id { get; set; }
        public string comcod { get; set; }
        public string rescode { get; set; }
        public string spcfcode { get; set; }
        public string refid { get; set; }
        public decimal rate { get; set; }
        public long currency_id { get; set; }
        public decimal currency_rate { get; set; }
        public string remarks { get; set; }
        public bool is_audit { get; set; }
        public DateTime audit_date { get; set; }
        public int audit_by { get; set; }
        public int num_of_client { get; set; }
        public decimal num_of_experience { get; set; }
        public string medium { get; set; }
        public int lead_time { get; set; }
        public DateTime created_date { get; set; }

        public VendorSuply(long sup_item_id_, string comcod_, string rescode_, string spcfcode_, string refid_, decimal rate_, long currency_id_, decimal currency_rate_, string remarks_, bool is_audit_, DateTime audit_date_, int audit_by_, int num_of_client_, decimal num_of_experience_, string medium_, int lead_time_, DateTime created_date_)
        {
            this.sup_item_id = sup_item_id_;
            this.comcod = comcod_;
            this.rescode = rescode_;
            this.spcfcode = spcfcode_;
            this.refid = refid_;
            this.rate = rate_;
            this.currency_id = currency_id_;
            this.currency_rate = currency_rate_;
            this.remarks = remarks_;
            this.is_audit = is_audit_;
            this.audit_date = audit_date_;
            this.audit_by = audit_by_;
            this.num_of_client = num_of_client_;
            this.num_of_experience = num_of_experience_;
            this.medium = medium_;
            this.lead_time = lead_time_;
            this.created_date = created_date_;
        }
    }
}
