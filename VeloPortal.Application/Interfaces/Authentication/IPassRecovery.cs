using VeloPortal.Domain.Entities.Authentication;

namespace VeloPortal.Application.Interfaces.Authentication
{
    public interface IPassRecovery
    {
        Task<bool> InsertOrUpdatePassRecovery(PassRecovery obj, string action);
        /// <summary>Latest recovery record of this user (id AND e-mail), never looked up by the code alone.</summary>
        Task<PassRecovery?> GetLatestForUserAsync(int userId, string email);
    }
}
