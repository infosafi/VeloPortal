using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.SystemConfig
{
    [Table("CompanyInf", Schema = "itv_sys")]
    [PrimaryKey(nameof(comcod))]
    public class CompanyInf
    {
        public string? comcod { get; set; }
        public string? company_name { get; set; }
        public string? short_name { get; set; }
        public string? addressline1 { get; set; }
        public string? addressline2 { get; set; }
        public string? company_phone { get; set; }
        public string? company_email { get; set; }
        public string? company_web { get; set; }
        public string? company_tin { get; set; }
        public string? company_bin { get; set; }
        public int company_type { get; set; }
        public bool is_multicurrency { get; set; }
        public int default_currency { get; set; }
        public string? company_overview { get; set; }
        public int seq { get; set; }
        public bool is_active { get; set; }
        public string? parent_company { get; set; } = "00000";
        public string? company_logo { get; set; }
        public string? favicon_logo { get; set; }
        public bool is_multilang { get; set; }
        public int default_lang { get; set; }
        public string? role_system { get; set; }
        public bool is_breadcrumb { get; set; }
        public bool is_strong_pass { get; set; }
        public string? value_format { get; set; }
        public int alert_style { get; set; }
        public DateTime opening_date { get; set; } = Convert.ToDateTime("01-Jan-1900");
        public int menubar { get; set; }
        public string? portal_url { get; set; }
        public CompanyInf() { }

        public CompanyInf(string comcod_, string company_name_, string short_name_, string addressline1_,
            string addressline2_, string company_phone_, string company_email_,
            string company_web_, string company_tin_, string company_bin_,
            int company_type_, bool is_multicurrency_,
            int default_currency_, string company_overview_,
            int seq_, bool is_active_, string parent_company_,
            string company_logo_, string favicon_logo_,
            bool is_multilang_, int default_lang_, string role_system_,
            bool is_breadcrumb, string portal_url_)
        {
            this.comcod = comcod_;
            this.company_name = company_name_;
            this.short_name = short_name_;
            this.addressline1 = addressline1_;
            this.addressline2 = addressline2_;
            this.company_phone = company_phone_;
            this.company_email = company_email_;
            this.company_web = company_web_;
            this.company_tin = company_tin_;
            this.company_bin = company_bin_;
            this.company_type = company_type_;
            this.is_multicurrency = is_multicurrency_;
            this.default_currency = default_currency_;
            this.company_overview = company_overview_;
            this.seq = seq_;
            this.is_active = is_active_;
            this.parent_company = parent_company_;
            this.company_logo = company_logo_;
            this.favicon_logo = favicon_logo_;
            this.is_multilang = is_multilang_;
            this.default_lang = default_lang_;
            this.role_system = role_system_;
            this.is_breadcrumb = is_breadcrumb;
            this.portal_url = portal_url_;
        }
    }
}
