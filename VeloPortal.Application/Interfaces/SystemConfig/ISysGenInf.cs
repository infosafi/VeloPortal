using VeloPortal.Domain.Entities.SystemConfig;

namespace VeloPortal.Application.Interfaces.SystemConfig
{
    public interface ISysGenInf
    {
        Task<IEnumerable<SysGenInf>?> GetSysGenInfListByStatusAndGenCode(string? comcod, string? gencod, bool? is_active);
        Task<IEnumerable<SysGenInf>?> GetSysGenInfGroupCode(string? comcod);
    }
}
