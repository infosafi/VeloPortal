using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.Authentication
{
    [Table("RefreshToken", Schema = "itv_auth")]
    [PrimaryKey(nameof(guid_id))]
    public class RefreshToken
    {
        public Guid guid_id { get; set; }
        public string? token { get; set; }
        public DateTime expires { get; set; }
        public bool is_revoked { get; set; }
        public long user_id { get; set; }
    }
}
