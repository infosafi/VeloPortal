namespace VeloPortal.Application.Interfaces.ProjectManagement
{
    public interface IUnitInfo
    {
        Task<IEnumerable<dynamic>?> GetUnitInfoByCodeAsync(string? comcod, string? acccode, string? unitType = "%", string? floor = "%");
    }
}
