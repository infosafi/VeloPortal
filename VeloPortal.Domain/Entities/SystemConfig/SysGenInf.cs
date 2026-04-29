using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.SystemConfig
{
    [Table("SysGenInf", Schema = "itv_sys")]
    [PrimaryKey(nameof(sys_gen_code_id), nameof(comcod), nameof(gencod))]
    public class SysGenInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long sys_gen_code_id { get; set; }
        public string? comcod { get; set; }
        public string? gencod { get; set; }
        public string? gendesc { get; set; }
        public string? short_desc { get; set; }
        public decimal genvalue { get; set; }
        public string? gentype { get; set; }

        public int seq { get; set; }
        public string? remarks { get; set; }
        public bool is_active { get; set; }
        public bool is_default { get; set; }
        public DateTime created_date { get; set; }
        public int created_by { get; set; }

        public SysGenInf() { }
        public SysGenInf(long sys_gen_code_id_, string comcod_, string gencod_, string gendesc_, string short_desc_, decimal genvalue_,
            int seq_, string remarks_, bool is_active_, bool is_default_, DateTime created_date_, int created_by_, string _gentype)
        {
            this.sys_gen_code_id = sys_gen_code_id_;
            this.comcod = comcod_;
            this.gencod = gencod_;
            this.gendesc = gendesc_;
            this.short_desc = short_desc_;
            this.genvalue = genvalue_;
            this.seq = seq_;
            this.remarks = remarks_;
            this.is_active = is_active_;
            this.is_default = is_default_;
            this.created_date = created_date_;
            this.created_by = created_by_;
            this.gentype = _gentype;
        }
    }
}
