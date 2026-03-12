using VeloPortal.Application.DTOs.Procurement;

namespace VeloPortal.Application.Interfaces.Procurement
{
    public interface ISupplierInf
    {
        Task<IEnumerable<DtoVendorSupply>?> GetSupplierSupplyItems(string? comcod);

        Task<bool> DeleteVendorSuplyById(int supItemId);
    }
}
