using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Procurement
{
    public class DtoPurOrderInfo
    {
        public List<PurOrderInfo> PurOrderInfo { get; set; }
        public List<PurOrderItem> PurOrderItems { get; set; }
        public List<PurOrderSchedule> PurOrderSchedule { get; set; }
        public List<PurOrderDoc> PurOrderDoc { get; set; }
    }

    public class PurOrderInfo
    {
        public string comcod { get; set; }
        public long pur_ord_id { get; set; }
        public string orderno { get; set; }
        public string refno { get; set; }
        public DateTime order_date { get; set; }
        public string rescode { get; set; }
        public string supplier { get; set; }
        public string sup_address { get; set; }
        public string sup_phone { get; set; }
        public decimal advance_amt { get; set; }
        public decimal vatamt { get; set; } 
        public decimal taxamt { get; set; }
        public string narration { get; set; }
        public string inco_terms { get; set; }
        public string payment_mode { get; set; }
        public DateTime estimated_delivery { get; set; }
        public string terms_conditions { get; set; }
    }

    public class PurOrderItem
    {
        public string comcod { get; set; }
        public int pur_ord_id { get; set; }
        public int pur_ord_item_id { get; set; }

        public string rescode { get; set; }
        public string resdesc { get; set; }

        public string spcfcode { get; set; }
        public string spcfdesc { get; set; }

        public string refcode { get; set; }

        public string gencode { get; set; }
        public string brand { get; set; }

        public string notes { get; set; }

        public decimal? order_qty { get; set; }
        public decimal? free_qty { get; set; }
        public decimal? rate { get; set; }
        public decimal? discount { get; set; }
        public decimal? amount { get; set; }

        public int? req_id { get; set; }
        public int? ord_app_id { get; set; }

        public DateTime? estimated_delivery { get; set; }
    }

    public class PurOrderSchedule
    {
        public string comcod { get; set; }
        public int pur_ord_id { get; set; }
        public int pur_ord_sch_id { get; set; }

        public string gencode { get; set; }
        public string installments { get; set; }

        public DateTime schedule_date { get; set; }
        public decimal schedule_amt { get; set; }

        public string notes { get; set; }
    }

    public class PurOrderDoc
    {

        public string comcod { get; set; }
        public long docid { get; set; }
        public string refno { get; set; }
        public string remarks { get; set; }
        public string doc_title { get; set; }
        public string doc_url { get; set; }
        public string doc_type { get; set; }
        public long doc_size { get; set; }
    }
}
