using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.SystemConfig
{
    [Table("AccCodeInf", Schema = "itv_sys")]
    [PrimaryKey(nameof(acc_id), nameof(comcod), nameof(acccode))]
    public class AccCodeInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int acc_id { get; set; }
        public string? comcod { get; set; }
        public string? acccode { get; set; }
        public string? accdesc { get; set; }
        public string? shortdesc { get; set; }
        public string? res_link_code { get; set; }
        public int seq { get; set; }
        public string? acc_gen_code { get; set; }
        public string? cash_flow_code { get; set; }
        public int created_by { get; set; }
        public bool is_active { get; set; }
        public DateTime created_date { get; set; }
        public bool is_default { get; set; }
        public bool is_manual_post { get; set; }
        public DateTime closing_date { get; set; }
        public bool is_has_resources { get; set; }


        public AccCodeInf() { }

        public AccCodeInf(int acc_id_, string comcod_, string acccode_, string accdesc_,
            string shortdesc_, string res_link_code_, int seq_, string acc_gen_code_,
            string cash_flow_code_, int created_by_, bool is_active_, DateTime created_date_, bool is_default_,
            bool is_manual_post_, DateTime closing_date, bool is_has_resources)
        {
            this.acc_id = acc_id_;
            this.comcod = comcod_;
            this.acccode = acccode_;
            this.accdesc = accdesc_;
            this.shortdesc = shortdesc_;
            this.res_link_code = res_link_code_;
            this.seq = seq_;
            this.acc_gen_code = acc_gen_code_;
            this.cash_flow_code = cash_flow_code_;
            this.created_by = created_by_;
            this.is_active = is_active_;
            this.created_date = created_date_;
            this.is_default = is_default_;
            this.is_manual_post = is_manual_post_;
            this.closing_date = closing_date;
            this.is_has_resources = is_has_resources;
        }
    }
}
