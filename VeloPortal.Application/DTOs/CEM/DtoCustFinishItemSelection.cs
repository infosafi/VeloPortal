using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.CEM
{
    public class DtoCustFinishItemSelection
    {
        public dynamic? Info { get; set; }
        public IEnumerable<dynamic>? Items { get; set; }
    }
}
