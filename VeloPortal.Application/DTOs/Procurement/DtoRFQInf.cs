using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Procurement
{
    public class DtoRFQInf
    {
        public DtoRFQDetails? DtoRFQDetails { get; set; }
        public List<DtoRFQItems>? DtoRFQItems { get; set; }
        public List<DtoRFQBidder>? DtoRFQBidder { get; set; }
    }
}
