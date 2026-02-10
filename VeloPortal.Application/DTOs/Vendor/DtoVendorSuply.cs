namespace VeloPortal.Application.DTOs.Vendor
{
    public class DtoVendorSuply
    {
        public long sup_item_id { get; set; }
        public string? comcod { get; set; }
        public string? rescode { get; set; }
        public string? resdesc { get; set; }
        public string? spcfcode { get; set; }
        public string? spec_desc { get; set; }
        public string? refid { get; set; }
        public decimal rate { get; set; }
        public string? currency_id { get; set; }
        public decimal currency_rate { get; set; }
        public int num_of_client { get; set; }
        public int num_of_experience { get; set; }
        public DateTime created_date { get; set; }
        public string? remarks { get; set; }
        public bool is_audit { get; set; }
        public DateTime? audit_date { get; set; }
        public string? audit_by { get; set; }
        public string? audit_by_name { get; set; }
        public string? medium { get; set; }
        public string? lead_time { get; set; }
    }
}
