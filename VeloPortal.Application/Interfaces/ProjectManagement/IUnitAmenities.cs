using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloPortal.Application.Interfaces.ProjectManagement
{
    public interface IUnitAmenities
    {
        Task<IEnumerable<dynamic>> GetUnitAmenitiesAsync(string comcod, long unitid = 0, string acccode = "", string rescode = "");

    }
}
