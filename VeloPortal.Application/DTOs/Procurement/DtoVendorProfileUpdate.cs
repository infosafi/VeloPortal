namespace VeloPortal.Application.DTOs.Procurement
{
    /// <summary>
    /// Fields a vendor may edit on their OWN profile. Deliberately excludes the identity and
    /// status fields (vendor_profile_id, comcod, vendorid, vendor_email, vendor_password, rescode,
    /// is_verify_acc, is_email_verify, is_approved, is_hold, is_audit): those are set by the server,
    /// the e-mail OTP flow or back-office staff only.
    /// </summary>
    public class DtoVendorProfileUpdate
    {
        public string? company_name { get; set; }
        public string? address { get; set; }
        public string? compan_overview { get; set; }
        public string? company_bin { get; set; }
        public string? contact_no { get; set; }
        public string? license_no { get; set; }
        public int? num_of_client { get; set; }
        public int? ong_num_of_client { get; set; }
        public string? contact_person { get; set; }
        public string? secondary_contact_no { get; set; }
        public string? designation { get; set; }
        public bool? is_available { get; set; }
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
        public string? user_photo { get; set; }
    }
}
