using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.Interfaces.Procurement
{
    public interface IPurRFQInf
    {
        Task<IEnumerable<dynamic>?> GetSingleRFQList(string? comcod, string? rfqid);
    }
}
