using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.FacilityManagement
{
    [Table("ServReqInf", Schema = "itv_fms")]
    [PrimaryKey(nameof(service_req_id))]
    public class ServReqInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long service_req_id { get; set; }
        public string comcod { get; set; } = String.Empty;
        public string service_no { get; set; } = String.Empty;
        public string acccode { get; set; } = String.Empty;
        public string unitcode { get; set; } = String.Empty;
        public string custcode { get; set; } = String.Empty;
        public int priority { get; set; }
        public DateTime service_req_date { get; set; }
        public DateTime est_done_date { get; set; }
        public string complain_details { get; set; } = String.Empty;
        public string special_notes { get; set; } = String.Empty;
        public int nursing_by { get; set; }
        public string req_medium { get; set; } = String.Empty;
        public int created_by { get; set; }
        public DateTime created_date { get; set; }
        public bool is_done { get; set; }
        public string current_step { get; set; } = String.Empty;
        public string customer_feedback { get; set; } = String.Empty;
        public int satisfaction { get; set; }

        public ServReqInf() { }
        public ServReqInf(long service_req_id_, string comcod_, string service_no_, string acccode_, string unitcode_, string custcode_, int priority_, DateTime service_req_date_, DateTime est_done_date_, string complain_details_, string special_notes_, int nursing_by_, string req_medium_, int created_by_, DateTime created_date_, bool is_done_, string current_step_, string customer_feedback_, int satisfaction_)
        {
            this.service_req_id = service_req_id_;
            this.comcod = comcod_;
            this.service_no = service_no_;
            this.acccode = acccode_;
            this.unitcode = unitcode_;
            this.custcode = custcode_;
            this.priority = priority_;
            this.service_req_date = service_req_date_;
            this.est_done_date = est_done_date_;
            this.complain_details = complain_details_;
            this.special_notes = special_notes_;
            this.nursing_by = nursing_by_;
            this.req_medium = req_medium_;
            this.created_by = created_by_;
            this.created_date = created_date_;
            this.is_done = is_done_;
            this.current_step = current_step_;
            this.customer_feedback = customer_feedback_;
            this.satisfaction = satisfaction_;
        }
    }
}
