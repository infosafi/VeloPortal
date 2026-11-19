using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Domain.Entities.AccountsFinance;

namespace VeloPortal.Application.Interfaces.SystemConfig
{
    public interface IAccCodeInf
    {
        Task<IEnumerable<dynamic>?> GetUserWiseAccCodeBookInfo(string? comcod, string? label, string? search_query, int user_id, bool? is_manual_post, bool? is_last_head);
        Task<IEnumerable<AccGenInf>?> GetAccGenInfListByStatusAndGenCode(string? comcod, string? gencod, bool? is_active);
        Task<IEnumerable<AccGenInf>?> GetAccGenInfGroupCode(string? comcod);

    }
}
