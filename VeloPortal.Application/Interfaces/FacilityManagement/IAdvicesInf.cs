using VeloPortal.Domain.Entities.FacilityManagement;

namespace VeloPortal.Application.Interfaces.FacilityManagement
{
    public interface IAdvicesInf
    {
        // Insert or Update Advices Information
        Task<List<AdvicesInf>?> InsertOrUpdateAdvicesInf(List<AdvicesInf> obj);

        // Get Advices Information List
        Task<IEnumerable<dynamic>?> GetAdviceInfList(string? comcod, string? fromdate, string? todate, string? rescode, string? type);
    }
}
