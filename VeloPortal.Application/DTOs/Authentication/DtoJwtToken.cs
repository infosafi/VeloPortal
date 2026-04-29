using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoJwtToken
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? DeviceId { get; set; } // Optional: for tracking device-specific tokens
    }
}
