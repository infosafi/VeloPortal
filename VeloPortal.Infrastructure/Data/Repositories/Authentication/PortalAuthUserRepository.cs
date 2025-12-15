using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VeloPortal.Application.DTOs.Common;
using VeloPortal.Application.Interfaces.Authentication;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Authentication
{
    public class PortalAuthUserRepository : IPortalAuthUser
    {

        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public PortalAuthUserRepository(
          IDbContextFactory<VeloPortalDbContext> dbContextFactory,
          IMapper mapper,
          IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public Task<DtoPortalAuthUser?> ValidateCredentialsAsync(string? comcod, string? user_or_email, string? password)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    //var userinfo = await dbContext.UserInf.FirstOrDefaultAsync(p => (p.user_email == user_or_email || p.user_name == user_or_email)
                    // && p.password == password);



                    //return _mapper.Map<DtoPortalAuthUser>(userinfo);

                    return null;
                }

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return null;
            }
        }

    }
}
