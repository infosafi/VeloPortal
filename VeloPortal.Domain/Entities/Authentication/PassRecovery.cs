using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.Authentication
{
    [Table("PassRecovery", Schema = "itv_auth")]
    [PrimaryKey(nameof(recover_id))]
    public class PassRecovery
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int recover_id { get; set; }
        public int user_id { get; set; }
        public string? user_email { get; set; }
        public string? recovery_otp { get; set; }
        public DateTime created_date { get; set; }
        public DateTime expiry_date { get; set; }
        public DateTime execution_date { get; set; }
        public string? request_agent { get; set; }
        public string? ip_address { get; set; }
        public bool is_recovered { get; set; }
        public bool is_expired { get; set; }
        public string? portal_role { get; set; }
    }
}
