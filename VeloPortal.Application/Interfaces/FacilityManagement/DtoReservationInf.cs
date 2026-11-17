using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.Interfaces.FacilityManagement
{
    public class DtoReservationInf
    {
        // Table 0: Getting Reservation Request Information
        public IEnumerable<dynamic>? Reservation { get; set; }

        // Table 1: Getting Property Amenities List
        public IEnumerable<dynamic>? Amenities { get; set; }
    }
    public class DtoReqAmenitiesSave
    {
        public long booking_amenities_id { get; set; }
        public long room_booking_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string gencode { get; set; } = string.Empty;
        public string frequncy { get; set; } = string.Empty;
        public decimal charge_amt { get; set; }
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public bool is_cancel { get; set; }
    }
}
