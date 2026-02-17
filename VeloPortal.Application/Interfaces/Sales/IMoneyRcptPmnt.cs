using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.Interfaces.Sales
{
    public interface IMoneyRcptPmnt
    {
        Task<IEnumerable<dynamic>?> GetUnitPaymentScheduleWithBalanceAsync( string comcod, string acccode, string urescode );
    }
}
