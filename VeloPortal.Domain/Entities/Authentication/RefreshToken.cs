using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Domain.Entities.Authentication
{
    public class RefreshToken
    {
        public Guid guid_id { get; set; }
        public string? token { get; set; }
        public DateTime expires { get; set; }
        public bool is_revoked { get; set; }
        public long user_id { get; set; }
    }
}
