using AutoMapper;
using VeloPortal.Application.DTOs.SystemConfig;
using VeloPortal.Domain.Entities.SystemConfig;

namespace VeloPortal.WebApi.Mappings
{
    public class SystemConfigMapper : Profile
    {
        public SystemConfigMapper()
        {
            CreateMap<ResCodeInf, DtoResCodeInf>().ReverseMap();
            CreateMap<ComApiInf, DtoComApiInf>().ReverseMap();
        }
    }
}
