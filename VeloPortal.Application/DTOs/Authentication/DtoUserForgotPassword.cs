using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.DTOs.Authentication
{
    public class DtoUserForgotPassword
    {
        public string? email_or_phone { get; set; } = String.Empty;
    }
}
