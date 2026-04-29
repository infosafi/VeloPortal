namespace VeloPortal.Application.DTOs.Procurement
{
    public class DtoRFQItems
    {
        public long pur_rfq_item_id { get; set; }
        public long pur_rfq_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string rescode { get; set; } = string.Empty;
        public string spcfcod { get; set; } = string.Empty;
        public string supcode { get; set; } = string.Empty;
        public bool is_include_vat { get; set; }
        public decimal rfq_qty { get; set; }
        public decimal quality_percent { get; set; }
        public decimal rate { get; set; }
        public decimal disc_percent { get; set; }
        public decimal vat_amount { get; set; }
        public decimal warranty { get; set; }
        public decimal current_stock { get; set; }
        public int leadtime { get; set; }
    }
}
