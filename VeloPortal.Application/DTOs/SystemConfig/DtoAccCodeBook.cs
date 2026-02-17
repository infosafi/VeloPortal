using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.SystemConfig
{
    public class DtoAccCodeBook
    {
        public int acc_id { get; set; }
        public string? comcod { get; set; }
        public string? acccode { get; set; }
        public string? accdesc { get; set; }
        public string? shortdesc { get; set; }
        public string? res_link_code { get; set; }
        public int seq { get; set; }
        public string? acc_gen_code { get; set; }
        public string? cash_flow_code { get; set; }
        public string? gendesc { get; set; }
        public string? cashdesc { get; set; }
        public int created_by { get; set; }
        public bool is_active { get; set; }
        public DateTime created_date { get; set; }
        public bool is_default { get; set; }
        public bool is_manual_post { get; set; }
        public bool is_permitable { get; set; }
        public bool is_permitted { get; set; }
        public bool is_has_resources { get; set; }


    }


    public class DtoAccId
    {

        public string? comcod { get; set; }
        public int acc_id { get; set; }
    }
}
