using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Entities.Authentication
{

    [Table("LoginLogs", Schema = "itv_auth")]
    [PrimaryKey(nameof(logid))]
    public class LoginLogs
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long logid { get; set; }
        public string? comcod { get; set; }
        public int userid { get; set; }
        public string? username_or_email { get; set; }
        public bool login_status { get; set; }
        public string? failure_reason { get; set; }
        public string? ip_address { get; set; }
        public string? user_agent { get; set; }
        public string? mac_address { get; set; }
        public string? remarks { get; set; }
        public string? usession_id { get; set; }
        public string? location { get; set; }
        public DateTime attempt_time { get; set; } = new DateTime(1900, 1, 1); //DateTimeExtensions.GetLocalTimeFromBaseOnTimeZone();

        public LoginLogs() { }

        public LoginLogs(long logid_, string? comcod_, int userid_, string username_or_email_, bool login_status_, string failure_reason_, string ip_address_, string user_agent_, string mac_address_, string remarks_, string usession_id_, string location_)
        {
            this.logid = logid_;
            this.comcod = comcod_;
            this.userid = userid_;
            this.username_or_email = username_or_email_;
            this.login_status = login_status_;
            this.failure_reason = failure_reason_;
            this.ip_address = ip_address_;
            this.user_agent = user_agent_;
            this.mac_address = mac_address_;
            this.remarks = remarks_;
            this.usession_id = usession_id_;
            this.location = location_;
        }
    }
}
