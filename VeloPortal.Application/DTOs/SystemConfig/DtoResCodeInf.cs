using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.SystemConfig
{
    public class DtoResCodeInf
    {

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
        public bool is_manual_post { get; set; }
        public bool is_default { get; set; }
        public bool is_permitable { get; set; }
        public bool is_permitted { get; set; }
        public bool is_taxable { get; set; }
        public decimal tax_percent { get; set; }
        public decimal west_percent { get; set; }


    }

    public class DtoResId
    {

        public string? comcod { get; set; }
        public int res_id { get; set; }
    }
}
