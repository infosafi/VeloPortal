using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Application.DTOs.CEM;
using VeloPortal.Application.DTOs.Helper;
using VeloPortal.Domain.Entities.CEM;

namespace VeloPortal.Application.Interfaces.CEM
{
    public interface IPrjStdSelection
    {
        Task<DtoCustFinishItemSelection?> GetCustomerFinishingItemSelection(string comcod, long? sale_unit_id);
        Task<DtoResponseHelper> InsertOrUpdateProjStdItemSelections(List<PrjStdSelection> PrjStdSelection);
    }
}
