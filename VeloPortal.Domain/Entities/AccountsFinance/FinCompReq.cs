using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.AccountsFinance
{
    [Table("FinCompReq", Schema = "itv_acc")]
    [PrimaryKey(nameof(fin_comp_req_id))]
    public class FinCompReq
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long fin_comp_req_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string fcrno { get; set; } = string.Empty;
        public string acccode { get; set; } = string.Empty;
        public string rescode { get; set; } = string.Empty;
        public string custcode { get; set; } = string.Empty;
        public string reqtype { get; set; } = string.Empty;
        public DateTime reqdate { get; set; }
        public string remarks { get; set; } = string.Empty;
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public bool is_cancel { get; set; }
        public bool is_approved { get; set; }
        public string review_note { get; set; } = string.Empty;
        public DateTime review_date { get; set; }
        public DateTime delivery_before { get; set; }
        public int review_by { get; set; }
        public string req_source { get; set; } = string.Empty;

        public FinCompReq() { }
        public FinCompReq(long fin_comp_req_id_, string comcod_, string fcrno_, string acccode_, string rescode_, string custcode_, string reqtype_, string remarks_, DateTime created_date_, int created_by_, bool is_cancel_, bool is_approved_, string review_note_, DateTime review_date_, DateTime delivery_before_, int review_by_)
        {
            this.fin_comp_req_id = fin_comp_req_id_;
            this.comcod = comcod_;
            this.fcrno = fcrno_;
            this.acccode = acccode_;
            this.rescode = rescode_;
            this.custcode = custcode_;
            this.reqtype = reqtype_;
            this.remarks = remarks_;
            this.created_date = created_date_;
            this.created_by = created_by_;
            this.is_cancel = is_cancel_;
            this.is_approved = is_approved_;
            this.review_note = review_note_;
            this.review_date = review_date_;
            this.delivery_before = delivery_before_;
            this.review_by = review_by_;
        }
    }
}
