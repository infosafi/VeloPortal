using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.Interfaces.Procurement
{
    public interface IPurOrderInf
    {
        // Get Purchase Order List
        Task<IEnumerable<dynamic>?> GetPurchaseOrderList(string? comcod, string? fromdate, string? todate, string? supplier);
    }
}
