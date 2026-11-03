namespace VeloPortal.Application.Interfaces.SystemConfig
{
    public interface IAppManagement
    {
        // Get Apps Information
        Task<IEnumerable<dynamic>?> GetAppsFeatAuth(string? appid, string? app_feature_id, string? comcode, string? app_object);
    }
}
