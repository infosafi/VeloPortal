using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Entities.CEM
{
    [Table("PrjCustSelection", Schema = "itv_cem")]
    [PrimaryKey(nameof(proj_cust_item_sel_id), nameof(fin_item_sel_req_id))]
    public class PrjCustSelection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long proj_cust_item_sel_id { get; set; }
        public long fin_item_sel_req_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public long sale_unit_id { get; set; }
        public string rescode { get; set; } = string.Empty;
        public string spcfcode { get; set; } = string.Empty;
        public int code_id { get; set; }
        public string gencode { get; set; } = string.Empty;
        public string component { get; set; } = string.Empty;
        public bool is_cust_selection { get; set; }
        public bool is_cust_supply { get; set; }
        public string remarks { get; set; } = string.Empty;
        public DateTime created_date { get; set; }
        public int created_by { get; set; }

        public PrjCustSelection() { }

        public PrjCustSelection(long proj_cust_item_sel_id_, long fin_item_sel_req_id_, string comcod_, bool is_cust_selection_, bool is_cust_supply_, string remarks_, DateTime created_date_, int created_by_, long sale_unit_id_)
        {
            this.proj_cust_item_sel_id = proj_cust_item_sel_id_;
            this.fin_item_sel_req_id = fin_item_sel_req_id_;
            this.comcod = comcod_;
            this.is_cust_selection = is_cust_selection_;
            this.is_cust_supply = is_cust_supply_;
            this.remarks = remarks_;
            this.created_date = created_date_;
            this.created_by = created_by_;
            this.sale_unit_id = sale_unit_id_;
        }
    }
}
