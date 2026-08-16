using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.SystemConfig
{
    [Table("ComApiInf", Schema = "itv_sys")]
    [PrimaryKey(nameof(api_id), nameof(comcod), nameof(gencode))]
    public class ComApiInf
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int api_id { get; set; }
        public string? comcod { get; set; }
        public string? gencode { get; set; }
        public string? apiname { get; set; }
        public string? apiurl { get; set; }
        public string? apikey { get; set; }
        public string? apisecret { get; set; }
        public string? access_token { get; set; }
        public string? additional1 { get; set; }
        public string? remarks { get; set; }
        public string? example { get; set; }
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public bool is_active { get; set; }
        public ComApiInf() { }
        public ComApiInf(int api_id_, string comcod_, string gencode_, string apiname_, string apiurl_, string apikey_, string apisecret_, string access_token_, string additional1_, string remarks_, string example_, DateTime created_date_, int created_by_, bool is_active_)
        {
            this.api_id = api_id_;
            this.comcod = comcod_;
            this.gencode = gencode_;
            this.apiname = apiname_;
            this.apiurl = apiurl_;
            this.apikey = apikey_;
            this.apisecret = apisecret_;
            this.access_token = access_token_;
            this.additional1 = additional1_;
            this.remarks = remarks_;
            this.example = example_;
            this.created_date = created_date_;
            this.created_by = created_by_;
            this.is_active = is_active_;
        }
    }
}
