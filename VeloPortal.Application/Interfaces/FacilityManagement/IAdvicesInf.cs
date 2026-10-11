using VeloPortal.Domain.Entities.FacilityManagement;

namespace VeloPortal.Application.Interfaces.FacilityManagement
{
    public interface IAdvicesInf
    {
        // Insert or Update Apps License Information
        Task<List<AdvicesInf>?> InsertOrUpdateAdvicesInf(List<AdvicesInf> obj);
    }
}
