using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
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
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public PortalAuthUserRepository(
          IDbContextFactory<VeloPortalDbContext> dbContextFactory,
          IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<DtoUserInf?> ValidateCredentialsAsync(string comcod, string user_type, string user_or_email, string password)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<DtoUserInf?>(null);
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod,"itv_portal.SP_USER_OPERATION", "Get_Auth_User_Info", user_type, user_or_email, password);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<DtoUserInf?>(null);
                }

                var userList = ds.Tables[0].DataTableToList<DtoUserInf>();
                var user = userList?.FirstOrDefault();

                return await Task.FromResult(user);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<DtoUserInf?>(null);
            }
        }
    }
}
