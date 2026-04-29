using VeloPortal.Application.DTOs.Procurement;

namespace VeloPortal.Application.Interfaces.Procurement
{
    public interface IPurOrderInf
    {
        // Get Purchase Order List
        Task<IEnumerable<dynamic>?> GetPurchaseOrderList(string? comcod, string? fromdate, string? todate, string? supplier);

        // Get Purchase Order Information based on company code, purchase order ID, and order number
        Task<DtoPurOrderInfo?> GetPurchaseOrderInfo(string? comcod, string? pur_ord_id, string? orderno);
    }
}
