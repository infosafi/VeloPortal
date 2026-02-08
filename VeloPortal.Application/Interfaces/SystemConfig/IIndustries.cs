using VeloPortal.Domain.Entities.SystemConfig;

namespace VeloPortal.Application.Interfaces.SystemConfig
{
    public interface IIndustries
    {
        Task<IEnumerable<Industries>?> GetCompanyIndustries(bool? is_active);
    }
}
