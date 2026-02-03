using VeloPortal.Application.DTOs.SystemConfig;
using VeloPortal.Domain.Entities.SystemConfig;

namespace VeloPortal.Application.Interfaces.SystemConfig
{
    public interface IResCodeInf
    {
        Task<IEnumerable<DtoResCodeInf>?> GetRescodeInfListByStatusAndCode(string? comcod, string? rescode, bool? is_active);
        Task<IEnumerable<ResCodeInf>?> GetRescodeInfBookGroupCode(string? comcod);
        Task<IEnumerable<dynamic>?> GetUserWiseResCodeBookInfo(string? comcod, string? label, string? search_query, int user_id, bool? is_manual_post, bool? is_last_head);
    }
}
