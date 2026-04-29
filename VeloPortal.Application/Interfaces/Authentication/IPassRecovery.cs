using VeloPortal.Domain.Entities.Authentication;

namespace VeloPortal.Application.Interfaces.Authentication
{
    public interface IPassRecovery
    {
        Task<bool> InsertOrUpdatePassRecovery(PassRecovery obj, string action);
        Task<PassRecovery?> GetLatestOtpAsync(string? otp);
    }
}
