using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Entities.FacilityManagement
{
    [Table("ReservationInf", Schema = "itv_fms")]
    [PrimaryKey(nameof(room_booking_id))]
    public class ReservationInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long room_booking_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string acccode { get; set; } = string.Empty;
        public string rescode { get; set; } = string.Empty;
        public string requester_role { get; set; } = string.Empty;
        public DateTime reqdate { get; set; }
        public DateTime fromdate { get; set; }
        public DateTime todate { get; set; }
        public string purpose { get; set; } = string.Empty;
        public string remarks { get; set; } = string.Empty;
        public int presense { get; set; }
        public string reqsource { get; set; } = string.Empty;
        public bool is_approved { get; set; }
        public bool is_cancel { get; set; }
        public DateTime created_date { get; set; }
        public long created_by { get; set; }
        public DateTime review_date { get; set; }
        public int review_by { get; set; }
        public string review_note { get; set; } = string.Empty;
        public string host_type { get; set; } = string.Empty;
        public long booking_host { get; set; }
        public decimal booking_fee { get; set; }

        public ReservationInf() { }
        public ReservationInf(long room_booking_id_, string comcod_, string acccode_, string rescode_, string requester_role_, DateTime reqdate_, DateTime fromdate_, DateTime todate_, string purpose_, string remarks_, int presense_, string reqsource_, bool is_approved_, bool is_cancel_, DateTime created_date_, long created_by_, DateTime review_date_, int review_by_, string review_note_)
        {
            this.room_booking_id = room_booking_id_;
            this.comcod = comcod_;
            this.acccode = acccode_;
            this.rescode = rescode_;
            this.requester_role = requester_role_;
            this.reqdate = reqdate_;
            this.fromdate = fromdate_;
            this.todate = todate_;
            this.purpose = purpose_;
            this.remarks = remarks_;
            this.presense = presense_;
            this.reqsource = reqsource_;
            this.is_approved = is_approved_;
            this.is_cancel = is_cancel_;
            this.created_date = created_date_;
            this.created_by = created_by_;
            this.review_date = review_date_;
            this.review_by = review_by_;
            this.review_note = review_note_;
        }
    }
}
