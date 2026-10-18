namespace VeloPortal.Application.DTOs.Procurement
{
    public class DtoPeriodicRfqlist
    {
        public long pur_rfq_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string rfqno { get; set; } = string.Empty;
        public string refno { get; set; } = string.Empty;
        public DateTime rfqdate { get; set; }
        public string payment_mode { get; set; } = string.Empty;
        public bool is_include_vat { get; set; }
        public DateTime expire_date { get; set; }
        public decimal advance_percent { get; set; }
        public string terms_conditions { get; set; } = string.Empty;
        public string remarks { get; set; } = string.Empty;
        public DateTime created_date { get; set; }
        public string created_by { get; set; } = string.Empty;  
        public int items { get; set; }
        public decimal rfq_qty { get; set; }
        public decimal advance_amt { get; set; }
        public decimal credit_day { get; set; }
        public string supcode { get; set; } = string.Empty;
        public string supdesc { get; set; } = string.Empty;
        public bool is_submit { get; set; }
    }
}
