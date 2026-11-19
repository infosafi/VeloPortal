using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Entities.AccountsFinance
{
    [Table("AccGenInf", Schema = "itv_acc")]
    [PrimaryKey(nameof(comcod), nameof(acc_gen_id), nameof(gencode))]
    public class AccGenInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long acc_gen_id { get; set; }
        public string? comcod { get; set; }
        public string? gencode { get; set; }
        public string? gendesc { get; set; }
        public string? short_desc { get; set; }
        public string? remarks { get; set; }
        public bool is_default { get; set; }
        public bool is_active { get; set; }
        public int seq { get; set; }
        public int created_by { get; set; }
        public DateTime created_date { get; set; }
        public string? gentype { get; set; } = string.Empty;
        public string options { get; set; } = string.Empty;
        public decimal genvalue { get; set; }
        public AccGenInf() { }

        public AccGenInf(long acc_gen_id_, string comcod_, string gencode_, string gendesc_, string remarks_, bool is_default_, bool is_active_, int seq_, int created_by_, DateTime created_date_, string _gentype, string _options, decimal genvalue_)
        {
            this.acc_gen_id = acc_gen_id_;
            this.comcod = comcod_;
            this.gencode = gencode_;
            this.gendesc = gendesc_;
            this.remarks = remarks_;
            this.is_default = is_default_;
            this.is_active = is_active_;
            this.seq = seq_;
            this.created_by = created_by_;
            this.created_date = created_date_;
            this.gentype = _gentype;
            this.options = _options;
            this.genvalue = genvalue_;
        }
    }
}
