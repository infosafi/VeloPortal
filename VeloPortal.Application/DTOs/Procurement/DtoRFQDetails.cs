namespace VeloPortal.Application.DTOs.Procurement
{
    public class DtoRFQDetails
    {
        public long pur_rfq_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string rfqno { get; set; } = string.Empty;
        public string refno { get; set; } = string.Empty;
        public DateTime rfqdate { get; set; }
        public DateTime expire_date { get; set; }
        public DateTime created_date { get; set; }
        public string payment_mode { get; set; } = string.Empty;
        public string terms_conditions { get; set; } = string.Empty;
        public string remarks { get; set; } = string.Empty;
        public bool is_include_vat { get; set; }
        public decimal advance_percent { get; set; }
        public decimal credit_day { get; set; }
        public int created_by { get; set; }
    }
}
