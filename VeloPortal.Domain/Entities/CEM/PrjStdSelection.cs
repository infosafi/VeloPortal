using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.CEM
{
    [Table("PrjStdSelection", Schema = "itv_cem")]
    [PrimaryKey(nameof(proj_items_sel_id))]
    public class PrjStdSelection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long proj_items_sel_id { get; set; }
        public string comcod { get; set; } = string.Empty;
        public string acccode { get; set; } = string.Empty;
        public string rescode { get; set; } = string.Empty;
        public string spcfcode { get; set; } = string.Empty;
        public long code_id { get; set; }
        public string gencode { get; set; } = string.Empty;
        public string component { get; set; } = string.Empty;
        public string remarks { get; set; } = string.Empty;
        public bool is_selection { get; set; }
        public bool is_pre_suggestion { get; set; }

        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public PrjStdSelection() { }

        public PrjStdSelection(long proj_items_sel_id_, string comcod_, string acccode_, string rescode_, string spcfcode_, long code_id_, string gencode_, string component_, string remarks_, bool is_selection_, bool is_default_or_selection_, DateTime created_date_, int created_by_)
        {
            this.proj_items_sel_id = proj_items_sel_id_;
            this.comcod = comcod_;
            this.acccode = acccode_;
            this.rescode = rescode_;
            this.spcfcode = spcfcode_;
            this.code_id = code_id_;
            this.gencode = gencode_;
            this.component = component_;
            this.remarks = remarks_;
            this.is_selection = is_selection_;
            this.created_date = created_date_;
            this.created_by = created_by_;
        }
    }
}
