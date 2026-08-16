using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.SystemConfig
{
    [Table("Messagelog", Schema = "itv_sys")]
    [PrimaryKey(nameof(message_id))]
    public class Messagelog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long message_id { get; set; }
        public string comcod { get; set; } = String.Empty;
        public long module_id { get; set; }
        public string message_type { get; set; } = String.Empty;
        public string gateway { get; set; } = String.Empty;
        public string message_body { get; set; } = String.Empty;
        public string receiver_name { get; set; } = String.Empty;
        public string receiver { get; set; } = String.Empty;
        public string reference { get; set; } = String.Empty;
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public bool is_status { get; set; }

        public Messagelog() { }
        public Messagelog(long message_id_, string comcod_, long module_id_, string message_type_, string gateway_, string message_body_, string receiver_name_, string receiver_, string reference_, DateTime created_date_, int created_by_, bool is_status)
        {
            this.message_id = message_id_;
            this.comcod = comcod_;
            this.module_id = module_id_;
            this.message_type = message_type_;
            this.gateway = gateway_;
            this.message_body = message_body_;
            this.receiver_name = receiver_name_;
            this.receiver = receiver_;
            this.reference = reference_;
            this.created_date = created_date_;
            this.created_by = created_by_;
            this.is_status = is_status;
        }
    }
}
