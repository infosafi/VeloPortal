using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.Interfaces.Sales
{
    public interface ICustomerInf
    {
        Task<bool> TogggleCustProfilePrivacy(string? comcod, long? custid, bool isPublic);
        Task<IEnumerable<dynamic>?> GetRelatedProjPubProfile(string? comcod, string? custcode);
    }
}