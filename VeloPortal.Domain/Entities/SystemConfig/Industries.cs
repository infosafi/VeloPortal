using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.Domain.Entities.SystemConfig
{
    [Table("Industries", Schema = "itv_sys")]
    [PrimaryKey(nameof(industry_id))]
    public class Industries
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int industry_id { get; set; }
        public string? industry_name { get; set; }
        public string remarks { get; set; } = String.Empty;
        public string project_codes { get; set; } = String.Empty;
        public DateTime created_date { get; set; } = DateTimeExtensions.GetLocalTimeFromBaseOnTimeZone();
        public bool is_active { get; set; } = false;

        public Industries() { }
        public Industries(int industry_id_, string industry_name_, string remarks_, DateTime created_date_, bool is_active_, string project_codes_)
        {
            this.industry_id = industry_id_;
            this.industry_name = industry_name_;
            this.remarks = remarks_;
            this.created_date = created_date_;
            this.is_active = is_active_;
            this.project_codes = project_codes_;
        }
    }
}
