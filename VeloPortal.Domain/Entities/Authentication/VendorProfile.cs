using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.Authentication
{
    [Table("VendorProfile", Schema = "itv_scm")]
    [PrimaryKey(nameof(vendor_profile_id))]
    public class VendorProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int vendor_profile_id { get; set; }
        public string comcod { get; set; }
        public string? vendorid { get; set; }
        [StringLength(200)]
        public string? company_name { get; set; }
        public string? address { get; set; }
        public string? compan_overview { get; set; }
        public string? company_bin { get; set; }
        [StringLength(20)]
        public string? contact_no { get; set; }
        [Required]
        [EmailAddress]
        public string vendor_email { get; set; }
        public string? license_no { get; set; }
        public int? num_of_client { get; set; }
        public int? ong_num_of_client { get; set; }
        public string? contact_person { get; set; }
        public string? secondary_contact_no { get; set; }
        public string? designation { get; set; }
        [Required]
        public string vendor_password { get; set; }
        public bool is_available { get; set; } = true;
        public bool is_verify_acc { get; set; } = false;
        public bool is_email_verify { get; set; } = false;
        public decimal? experience { get; set; }
        public string? terms_condition { get; set; }
        public int? business_type { get; set; }
        public string? payment_mode { get; set; }
        public string? owner_name { get; set; }
        public string? owner_id_no { get; set; }
        public string? owner_tin_no { get; set; }
        public string? bankcode { get; set; }
        public string? branch { get; set; }
        public string? acc_name { get; set; }
        public string? acc_number { get; set; }
        public string? routeno { get; set; }
        public string? links { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public string? rescode { get; set; }
        public bool? is_audit { get; set; }
        public string? user_photo { get; set; }
        public bool is_hold { get; set; } = false;
        public bool is_approved { get; set; } = false;
    }
}
