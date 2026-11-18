using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Entities.ProjectImplementation
{
    [Table("ContractorProfile", Schema = "itv_prj")]
    [PrimaryKey(nameof(contractor_profile_id))]
    public class ContractorProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long contractor_profile_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string? contractorid { get; set; } = string.Empty;
        public string? company_name { get; set; } = string.Empty;
        public string? address { get; set; } = string.Empty;
        public string? compan_overview { get; set; } = string.Empty;
        public string? company_bin { get; set; } = string.Empty;
        public string? contact_no { get; set; } = string.Empty;
        public string? contractor_email { get; set; } = string.Empty;
        public string? license_no { get; set; } = string.Empty;
        public int? num_of_client { get; set; }
        public int? ong_num_of_client { get; set; }
        public string? contact_person { get; set; } = string.Empty;
        public string? secondary_contact_no { get; set; } = string.Empty;
        public string? designation { get; set; } = string.Empty;
        public string? contractor_password { get; set; } = string.Empty;
        public bool? is_available { get; set; } = true;
        public bool? is_verify_acc { get; set; } = false;
        public bool? is_email_verify { get; set; } = false;
        public decimal? experience { get; set; }
        public string? terms_condition { get; set; } = string.Empty;
        public int? business_type { get; set; }
        public string? payment_mode { get; set; } = string.Empty;
        public string? owner_name { get; set; } = string.Empty;
        public string? owner_id_no { get; set; } = string.Empty;
        public string? owner_tin_no { get; set; } = string.Empty;
        public string? bankcode { get; set; } = string.Empty;
        public string? branch { get; set; } = string.Empty;
        public string? acc_name { get; set; } = string.Empty;
        public string? acc_number { get; set; } = string.Empty;
        public string? routeno { get; set; } = string.Empty;
        public string? links { get; set; } = string.Empty;
        public DateTime? created_date { get; set; } = new DateTime(1900, 01, 01);
        public string? rescode { get; set; } = string.Empty;
        public bool is_audit { get; set; }
        public string? user_photo { get; set; } = string.Empty;
        public bool? is_hold { get; set; } = false;
        public bool? is_approved { get; set; } = false;
        public bool? is_rejected { get; set; } = false;

        public ContractorProfile() { }
        public ContractorProfile(long contractor_profile_id_, string comcod_, string contractorid_, string company_name_, string address_, string compan_overview_, string company_bin_, string contact_no_, string contractor_email_, string license_no_, int num_of_client_, int ong_num_of_client_, string contact_person_, string secondary_contact_no_, string designation_, string contractor_password_, bool is_available_, bool is_verify_acc_, bool is_email_verify_, decimal experience_, string terms_condition_, int business_type_, string payment_mode_, string owner_name_, string owner_id_no_, string owner_tin_no_, string bankcode_, string branch_, string acc_name_, string acc_number_, string routeno_, string links_, DateTime created_date_, string rescode_, bool is_audit_, string user_photo_, bool is_hold_, bool is_approved_ , bool is_rejected_)
        {
            this.contractor_profile_id = contractor_profile_id_;
            this.comcod = comcod_;
            this.contractorid = contractorid_;
            this.company_name = company_name_;
            this.address = address_;
            this.compan_overview = compan_overview_;
            this.company_bin = company_bin_;
            this.contact_no = contact_no_;
            this.contractor_email = contractor_email_;
            this.license_no = license_no_;
            this.num_of_client = num_of_client_;
            this.ong_num_of_client = ong_num_of_client_;
            this.contact_person = contact_person_;
            this.secondary_contact_no = secondary_contact_no_;
            this.designation = designation_;
            this.contractor_password = contractor_password_;
            this.is_available = is_available_;
            this.is_verify_acc = is_verify_acc_;
            this.is_email_verify = is_email_verify_;
            this.experience = experience_;
            this.terms_condition = terms_condition_;
            this.business_type = business_type_;
            this.payment_mode = payment_mode_;
            this.owner_name = owner_name_;
            this.owner_id_no = owner_id_no_;
            this.owner_tin_no = owner_tin_no_;
            this.bankcode = bankcode_;
            this.branch = branch_;
            this.acc_name = acc_name_;
            this.acc_number = acc_number_;
            this.routeno = routeno_;
            this.links = links_;
            this.created_date = created_date_;
            this.rescode = rescode_;
            this.is_audit = is_audit_;
            this.user_photo = user_photo_;
            this.is_hold = is_hold_;
            this.is_approved = is_approved_;
            this.is_rejected = is_rejected_;
        }
    }
}
