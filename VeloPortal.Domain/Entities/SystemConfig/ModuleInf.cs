using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeloPortal.Domain.Entities.SystemConfig
{
    [Table("ModuleInf", Schema = "itv_sys")]
    [PrimaryKey(nameof(module_id))]
    public class ModuleInf
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int module_id { get; set; }
        public int parent_mod_id { get; set; }
        public string? module_name { get; set; }
        public string? alias_name { get; set; }
        public string? module_prefix { get; set; }
        public string? remarks { get; set; }
        public string? covered_area { get; set; }
        public DateTime created_date { get; set; }

        public ModuleInf() { }
        public ModuleInf(int module_id_, int parent_mod_id_, string module_name_, string alias_name_, string module_prefix_, string remarks_, string covered_area_, DateTime created_date_)
        {
            this.module_id = module_id_;
            this.parent_mod_id = parent_mod_id_;
            this.module_name = module_name_;
            this.alias_name = alias_name_;
            this.module_prefix = module_prefix_;
            this.remarks = remarks_;
            this.covered_area = covered_area_;
            this.created_date = created_date_;
        }
    }
}
