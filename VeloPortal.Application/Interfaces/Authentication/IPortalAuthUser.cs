using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Application.DTOs.Common;

namespace VeloPortal.Application.Interfaces.Authentication
{
    public interface IPortalAuthUser
    {
        Task<DtoPortalAuthUser?> ValidateCredentialsAsync(string? comcod, string? user_or_email, string? password);
    }
}
