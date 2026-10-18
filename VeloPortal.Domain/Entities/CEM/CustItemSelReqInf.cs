using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Entities.CEM
{
    [Table("CustItemSelReqInf", Schema = "itv_cem")]
    [PrimaryKey(nameof(fin_item_sel_req_id))]
    public class CustItemSelReqInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long fin_item_sel_req_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public long sale_unit_id { get; set; }
        public DateTime reqdate { get; set; }
        public string remarks { get; set; } = string.Empty;
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public string req_source { get; set; } = string.Empty;
        public CustItemSelReqInf() { }

        public CustItemSelReqInf(long fin_item_sel_req_id_, string comcod_, long sale_unit_id_, DateTime reqdate_, string remarks_, DateTime created_date_, int created_by_, string req_source_)
        {
            this.fin_item_sel_req_id = fin_item_sel_req_id_;
            this.comcod = comcod_;
            this.sale_unit_id = sale_unit_id_;
            this.reqdate = reqdate_;
            this.remarks = remarks_;
            this.created_date = created_date_;
            this.created_by = created_by_;
            this.req_source = req_source_;
        }
    }
}
