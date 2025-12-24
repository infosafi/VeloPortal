using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.ServiceRequest
{
    public class DtoServiceRequestDetails
    {
        public IEnumerable<dynamic>? ServiceInfo { get; set; }
        public IEnumerable<dynamic>? ServiceProblemInfo { get; set; }
        public IEnumerable<dynamic>? ServiceResourceInfo { get; set; }
        public IEnumerable<dynamic>? ServiceTimelineInfo { get; set; }
        public IEnumerable<dynamic>? serviceExecution { get; set; }
    }
}
