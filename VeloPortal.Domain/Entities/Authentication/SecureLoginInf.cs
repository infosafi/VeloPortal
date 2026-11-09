using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.Authentication
{
    /// <summary>
    /// OTP store shared with the ERP (ERP 2FA rows have an empty/other user_type).
    /// Portal rows always use a dedicated user_type value (see SecureOtpRepository) so the two never mix.
    /// </summary>
    [Table("SecureLoginInf", Schema = "itv_auth")]
    [PrimaryKey(nameof(secure_login_id))]
    public class SecureLoginInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long secure_login_id { get; set; }
        public string comcod { get; set; } = String.Empty;
        public int user_id { get; set; }
        public string two_factor_otp { get; set; } = String.Empty;
        public DateTime create_date { get; set; }
        public DateTime expiry_date { get; set; }
        public DateTime execution_date { get; set; }
        public string ip_address { get; set; } = String.Empty;
        public string user_type { get; set; } = String.Empty;

        public SecureLoginInf() { }
    }
}
