using VeloPortal.Application.DTOs.SystemConfig;
using VeloPortal.Domain.Entities.SystemConfig;

namespace VeloPortal.Application.Interfaces.SystemConfig
{
    public interface IResCodeInf
    {
        Task<IEnumerable<DtoResCodeInf>?> GetRescodeInfListByStatusAndCode(string? comcod, string? rescode, bool? is_active);
        Task<IEnumerable<ResCodeInf>?> GetRescodeInfBookGroupCode(string? comcod);
    }
}
