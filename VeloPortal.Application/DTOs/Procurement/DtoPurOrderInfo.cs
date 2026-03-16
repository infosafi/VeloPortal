using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Procurement
{
    public class DtoPurOrderInfo
    {
        // Table 0: Purchase Order Information
        public IEnumerable<dynamic>? PurOrderInfo { get; set; }

        // Table 1: Purchase Order Items
        public IEnumerable<dynamic>? PurOrderItems { get; set; }

        // Table 2: Purchase Order Schedule
        public IEnumerable<dynamic>? PurOrderSchedule { get; set; }

        // Table 3: Purchase Order Attachment
        public IEnumerable<dynamic>? PurOrderDoc { get; set; }
    }
}
