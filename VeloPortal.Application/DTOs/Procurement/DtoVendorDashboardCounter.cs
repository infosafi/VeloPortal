namespace VeloPortal.Application.DTOs.Procurement
{
    public class DtoVendorDashboardCounter
    {
        public string? comcod { get; set; }
        public int quotation { get; set; }
        public int csenlist { get; set; }
        public int workorder { get; set; }
        public int supplyitems { get; set; }
        public decimal orderamount { get; set; }
        public decimal avgleadtime { get; set; }
        public decimal paymentreceived { get; set; }
        public int pendingdelivery { get; set; }
    }
}
