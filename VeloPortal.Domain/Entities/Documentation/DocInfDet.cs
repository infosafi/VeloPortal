using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.Documentation
{
    [Table("DocInfDet", Schema = "itv_doc")]
    [PrimaryKey(nameof(docid), nameof(comcod))]
    public class DocInfDet
    {
        public string? comcod { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long docid { get; set; }
        public long refid { get; set; }
        public string? refno { get; set; }
        public string? acccode { get; set; }
        public string? remarks { get; set; }
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public bool is_process { get; set; }
        public bool is_private { get; set; }
        public string? rescode { get; set; }
        public string? gencode { get; set; }
        public string? doc_title { get; set; }
        public string? doc_url { get; set; }
        public string? doc_type { get; set; }
        public string? doc_size { get; set; }
        public string? tags { get; set; }
        public bool is_download { get; set; }
        public bool is_expireble { get; set; }
        public DateTime expire_date { get; set; }

        public DocInfDet() { }
        public DocInfDet(string comcod_, long docid_, string refno_, string acccode_, string remarks_, DateTime created_date_, int created_by_, bool is_process_, bool is_private_, string rescode_, string gencode_, string doc_title_, string doc_url_, string doc_type_, string doc_size_, string tags_, bool is_download_, bool is_expireble_, DateTime expire_date_)
        {
            this.comcod = comcod_;
            this.docid = docid_;
            this.refno = refno_;
            this.acccode = acccode_;
            this.remarks = remarks_;
            this.created_date = created_date_;
            this.created_by = created_by_;
            this.is_process = is_process_;
            this.is_private = is_private_;
            this.rescode = rescode_;
            this.gencode = gencode_;
            this.doc_title = doc_title_;
            this.doc_url = doc_url_;
            this.doc_type = doc_type_;
            this.doc_size = doc_size_;
            this.tags = tags_;
            this.is_download = is_download_;
            this.is_expireble = is_expireble_;
            this.expire_date = expire_date_;
        }
    }
}
