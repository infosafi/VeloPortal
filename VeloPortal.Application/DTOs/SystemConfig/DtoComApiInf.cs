namespace VeloPortal.Application.DTOs.SystemConfig
{
    public class DtoComApiInf
    {
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
    }
}
