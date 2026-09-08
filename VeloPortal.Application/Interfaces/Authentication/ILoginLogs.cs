using VeloPortal.Domain.Entities.Authentication;

namespace VeloPortal.Application.Interfaces.Authentication
{
    public interface ILoginLogs
    {
        Task<bool> InsertOrUpdateLoginLogs(LoginLogs obj, string action);
    }
}
