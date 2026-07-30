using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.Interfaces.Sales
{
    public interface ISalesReport
    {
        Task<IEnumerable<dynamic>?> GetDelayCharge(string? comcod, string? fromdate, string? todate, string? acccode, string? rescode, string? custcode);
    }
}
