using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Application.Interfaces.CEM;
using VeloPortal.Domain.Entities.CEM;

namespace VeloPortal.Application.DTOs.CEM
{
    public class DtoPrjCustSelection
    {
        public CustItemSelReqInf CustItemSelReqInf { get; set; } = new();
        public List<PrjCustSelection> PrjCustSelection { get; set; } = new();
    }
}
