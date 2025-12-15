using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Application.DTOs.Common;

namespace VeloPortal.Application.Interfaces.Common
{
    public interface IJwtService
    {
        string GenerateAccessToken(DtoPortalUser? user);
        string GenerateRefreshToken();
    }
}
