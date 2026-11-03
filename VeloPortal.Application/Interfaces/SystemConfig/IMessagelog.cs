using VeloPortal.Application.DTOs.SystemConfig;

namespace VeloPortal.Application.Interfaces.SystemConfig
{
    public interface IMessagelog
    {
        Task<bool> SaveMessagelogAsync(DtoMessagelog dto);
    }
}
