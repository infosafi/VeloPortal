using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Application.DTOs.Sales;

namespace VeloPortal.Application.Interfaces.Sales
{
    public interface ICustomerInf
    {
        Task<bool> TogggleCustProfilePrivacy(DtoToggleCustProfilePrivacy obj);
        Task<IEnumerable<dynamic>?> GetRelatedProjPubProfile(string? comcod, string? custcode);
    }
}