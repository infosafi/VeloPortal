using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Entities.FacilityManagement
{
    [Table("ReqAmenities", Schema = "itv_fms")]
    [PrimaryKey(nameof(booking_amenities_id))]
    public class ReqAmenities
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long booking_amenities_id { get; set; }
        public long room_booking_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string gencode { get; set; } = string.Empty;
        public string frequncy { get; set; } = string.Empty;
        public decimal charge_amt { get; set; }
        public DateTime created_date { get; set; }
        public int created_by { get; set; }

        public ReqAmenities() { }

        public ReqAmenities(long booking_amenities_id_, long room_booking_id_, string comcod_, string gencode_, string frequncy_, decimal charge_amt_, DateTime created_date_, int created_by_)
        {
            this.booking_amenities_id = booking_amenities_id_;
            this.room_booking_id = room_booking_id_;
            this.comcod = comcod_;
            this.gencode = gencode_;
            this.frequncy = frequncy_;
            this.charge_amt = charge_amt_;
            this.created_date = created_date_;
            this.created_by = created_by_;
        }
    }
}
