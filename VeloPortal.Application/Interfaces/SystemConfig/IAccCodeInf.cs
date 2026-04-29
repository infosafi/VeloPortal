using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.Interfaces.SystemConfig
{
    public interface IAccCodeInf
    {
        Task<IEnumerable<dynamic>?> GetUserWiseAccCodeBookInfo(string? comcod, string? label, string? search_query, int user_id, bool? is_manual_post, bool? is_last_head);
    }
}
