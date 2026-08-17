namespace VeloPortal.Application.DTOs.SystemConfig
{
    public class DtoMessagelog
    {
        public string comcod { get; set; } = string.Empty;
        public long module_id { get; set; }
        public string message_type { get; set; } = string.Empty;
        public string gateway { get; set; } = string.Empty;
        public string message_body { get; set; } = string.Empty;
        public string receiver_name { get; set; } = string.Empty;
        public string receiver { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;
        public DateTime created_date { get; set; }
        public int created_by { get; set; }
        public bool is_status { get; set; }
        public string sources { get; set; } = String.Empty;
    }
}
