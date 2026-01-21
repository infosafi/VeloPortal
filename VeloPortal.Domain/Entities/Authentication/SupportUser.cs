using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.Authentication
{
    [Table("SupportUsers", Schema = "itv_fms")]
    [PrimaryKey(nameof(sup_user_id))]
    public class SupportUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long sup_user_id { get; set; }
        public string? comcod { get; set; }
        public string? fullname { get; set; }
        public string? username { get; set; }
        public string? suser_email { get; set; }
        public string? suser_phone { get; set; }
        public string? spassword { get; set; }
        public bool is_active { get; set; } = true;
        public string? gencode { get; set; }
        public int? created_by { get; set; }
        public DateTime created_date { get; set; } = DateTime.Now;
        public string? acccode { get; set; }
        public string? units { get; set; }
    }
}
