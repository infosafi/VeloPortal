using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.FacilityManagement
{

    [Table("AdvicesInf", Schema = "itv_fms")]
    [PrimaryKey(nameof(fms_message_id))]
    public class AdvicesInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long fms_message_id { get; set; }
        public string comcod { get; set; } = String.Empty;
        public string rescode { get; set; } = String.Empty;
        public string customer_role { get; set; } = String.Empty;
        public string message_subject { get; set; } = String.Empty;
        public string message_body { get; set; } = String.Empty;
        public string message_attached { get; set; } = String.Empty;
        public DateTime created_date { get; set; }
        public DateTime seen_date { get; set; }
        public int seen_by { get; set; }

        public AdvicesInf() { }

        public AdvicesInf(long fms_message_id_, string comcod_, string rescode_, string customer_role_, string message_subject_, string message_body_, string message_attached_, DateTime created_date_, DateTime seen_date_, int seen_by_)
        {
            this.fms_message_id = fms_message_id_;
            this.comcod = comcod_;
            this.rescode = rescode_;
            this.customer_role = customer_role_;
            this.message_subject = message_subject_;
            this.message_body = message_body_;
            this.message_attached = message_attached_;
            this.created_date = created_date_;
            this.seen_date = seen_date_;
            this.seen_by = seen_by_;
        }
    }
}
