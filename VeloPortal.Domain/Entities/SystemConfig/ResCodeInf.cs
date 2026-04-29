using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.SystemConfig
{
    [Table("ResCodeInf", Schema = "itv_sys")]
    [PrimaryKey(nameof(res_id), nameof(comcod), nameof(rescode))]
    public class ResCodeInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int res_id { get; set; }
        public string? comcod { get; set; }
        public string? rescode { get; set; }
        public string? resdesc { get; set; }
        public string? shortdesc { get; set; }
        public string? shortcode { get; set; }
        public string? additional_info { get; set; }
        public int seq { get; set; }
        public decimal res_value { get; set; }
        public decimal std_qty { get; set; }
        public string? res_unit { get; set; }
        public bool is_active { get; set; }
        public int created_by { get; set; }
        public string? remarks { get; set; }
        public DateTime created_date { get; set; }
        public string? link_rescode { get; set; }
        public string? spec_group { get; set; }
        public string? image_url { get; set; }
        public bool is_default { get; set; }
        public bool is_manual_post { get; set; }
        public bool is_taxable { get; set; }
        public decimal tax_percent { get; set; }
        public decimal west_percent { get; set; }


        public ResCodeInf() { }
        public ResCodeInf(int res_id_, string comcod_, string rescode_, string resdesc_,
            string shortdesc_, string shortcode_, string additional_info_, int seq_,
            decimal res_value_, decimal std_qty_, string res_unit_,
            bool is_active_, int created_by_, string remarks_,
            DateTime created_date_, string link_rescode_, string spec_group_, string image_url_, bool is_default_,
                bool is_manual_post_, bool is_taxable, decimal tax_percent, decimal west_percent)
        {

            this.res_id = res_id_;
            this.comcod = comcod_;
            this.rescode = rescode_;
            this.resdesc = resdesc_;
            this.shortdesc = shortdesc_;
            this.shortcode = shortcode_;
            this.additional_info = additional_info_;
            this.seq = seq_;
            this.res_value = res_value_;
            this.std_qty = std_qty_;
            this.res_unit = res_unit_;
            this.is_active = is_active_;
            this.created_by = created_by_;
            this.remarks = remarks_;
            this.created_date = created_date_;
            this.link_rescode = link_rescode_;
            this.spec_group = spec_group_;
            this.image_url = image_url_;
            this.is_default = is_default_;
            this.is_manual_post = is_manual_post_;
            this.is_taxable = is_taxable;
            this.tax_percent = tax_percent;
            this.west_percent = west_percent;

        }
    }
}
