namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoCustomer
    {
        public long sup_user_id { get; set; }
        public string comcod { get; set; }
        public string gencode { get; set; }
        public string fullname { get; set; }
        public string username { get; set; }
        public string suser_email { get; set; }
        public string suser_phone { get; set; }
        public string secondary_contact { get; set; }
        public string tertiary_contact { get; set; }
        public bool is_active { get; set; }
        public string acccode { get; set; }
        public string units { get; set; }
        public string user_role { get; set; }
        public string rolename { get; set; }
        public int created_by { get; set; }
        public DateTime created_date { get; set; }
    }
}
