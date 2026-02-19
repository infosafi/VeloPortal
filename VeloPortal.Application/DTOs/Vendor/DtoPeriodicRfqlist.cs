namespace VeloPortal.Application.DTOs.Vendor
{
    public class DtoPeriodicRfqlist
    {
        public int? pur_rfq_id { get; set; }
        public string? comcod { get; set; }
        public string? rfqno { get; set; }
        public string? refno { get; set; }
        public DateTime? rfqdate { get; set; }
        public string? payment_mode { get; set; }
        public bool? is_include_vat { get; set; }
        public DateTime? expire_date { get; set; }
        public decimal? advance_percent { get; set; }
        public string? terms_conditions { get; set; }
        public string? remarks { get; set; }
        public DateTime? created_date { get; set; }
        public string? created_by { get; set; }
        public string? items { get; set; }
        public decimal? rfq_qty { get; set; }
        public decimal? advance_amt { get; set; }
        public int? credit_day { get; set; }
        public string? supcode { get; set; }
        public bool? is_submit { get; set; }
    }
}
