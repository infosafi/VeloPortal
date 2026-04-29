using VeloPortal.WebApi.Mappings;

namespace VeloPortal.WebApi.Extensions
{
    public static class AutoMapperExtension
    {
        public static IServiceCollection AddMapperServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(SystemConfigMapper));
            return services;

        }
    }
}
