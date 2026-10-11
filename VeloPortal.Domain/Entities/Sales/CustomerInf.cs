using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Entities.Sales
{
    [Table("CustomerInf", Schema = "itv_sale")]
    [PrimaryKey(nameof(custid))]
    public class CustomerInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long custid { get; set; }
        public string? comcod { get; set; } = String.Empty;
        public string? rescode { get; set; } = String.Empty;
        public string? cust_name { get; set; } = String.Empty;
        public string? cust_code { get; set; } = String.Empty;
        public string? concern_person { get; set; } = String.Empty;
        public string? contact_no { get; set; } = String.Empty;
        public string secondary_contact { get; set; } = String.Empty;
        public string tertiary_contact { get; set; } = String.Empty;
        public string? cust_email { get; set; } = String.Empty;
        public string? cust_address { get; set; } = String.Empty;
        public decimal opening_amout { get; set; }
        public decimal limit { get; set; }
        public bool is_active { get; set; }
        public int created_by { get; set; }
        public DateTime created_date { get; set; }
        public string? cust_pass { get; set; } = String.Empty;
        public string? cust_photo { get; set; } = String.Empty;
        public string? cust_team { get; set; } = String.Empty;
        public bool is_enable_notification { get; set; }
        public string notify_contact { get; set; } = String.Empty;
        public long zone_id { get; set; }
        public bool is_public_for_app { get; set; }

        public CustomerInf() { }
        public CustomerInf(long custid_, string comcod_, string rescode_, string cust_name_, string cust_code_, string concern_person_, string contact_no_, string cust_email_, string cust_address_, decimal opening_amout_, decimal limit_, bool is_active_, DateTime created_date_, string cust_pass_, string cust_photo_, string cust_team_, bool is_public_for_app_)
        {
            this.custid = custid_;
            this.comcod = comcod_;
            this.rescode = rescode_;
            this.cust_name = cust_name_;
            this.cust_code = cust_code_;
            this.concern_person = concern_person_;
            this.contact_no = contact_no_;
            this.cust_email = cust_email_;
            this.cust_address = cust_address_;
            this.opening_amout = opening_amout_;
            this.limit = limit_;
            this.is_active = is_active_;
            this.created_date = created_date_;
            this.cust_pass = cust_pass_;
            this.cust_photo = cust_photo_;
            this.cust_team = cust_team_;
            this.is_public_for_app = is_public_for_app_;
        }
    }
}
