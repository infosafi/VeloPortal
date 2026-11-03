using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Domain.Entities.FacilityManagement;
using static VeloPortal.Application.Interfaces.FacilityManagement.IReservationInf;

namespace VeloPortal.Application.Interfaces.FacilityManagement
{
    public interface IReservationInf
    {
        // Get Reservation Information List based on company code, from date, to date, and account code
        Task<IEnumerable<dynamic>?> GetReservationInfList(string? comcod, string? fromDate, string? toDate, string? acccode);
        // Insert or Update Reservation Information
        Task<ReservationInf?> InsertOrUpdateReservationInf(ReservationInf obj);
    }
}
