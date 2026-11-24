using VeloPortal.Application.DTOs.Procurement;

namespace VeloPortal.Application.Interfaces.Procurement
{
    public interface ISupplierInf
    {
        Task<IEnumerable<DtoVendorSupply>?> GetSupplierSupplyItems(string? comcod, long? vendor_profile_id);

        Task<bool> DeleteVendorSupplyItemsById(string? comcod, int sup_item_id);
    }
}
