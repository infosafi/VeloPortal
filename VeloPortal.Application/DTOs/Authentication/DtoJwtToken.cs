using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoJwtToken
    {
        public string? comcod { get; set; } = string.Empty;
        public string? AccessToken { get; set; } 
        public string? RefreshToken { get; set; }
        public string? DeviceId { get; set; } // Optional: for tracking device-specific tokens
        public string? user_type { get; set; } = string.Empty;
        public string? user_role { get; set; } = string.Empty;
    }
}