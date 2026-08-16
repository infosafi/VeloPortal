using VeloPortal.Application.DTOs.SystemConfig;

namespace VeloPortal.Application.Interfaces.SystemConfig
{
    public interface IComApiInf
    {
        Task<IEnumerable<DtoComApiInf>?> GetCompanyAllComApiInf(string? comcod, string? gencode);
    }
}
