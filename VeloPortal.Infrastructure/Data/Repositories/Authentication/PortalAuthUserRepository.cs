using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using VeloPortal.Application.DTOs.Authentication;
using VeloPortal.Application.Interfaces.Authentication;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.SystemConfig;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.Repositories.FacilityManagement;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Authentication
{
    public class PortalAuthUserRepository : IPortalAuthUser
    {

        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServReqInfRepository> _logger;
        private readonly SPProcessAccess? _spProcessAccess;
  

        public PortalAuthUserRepository(
          IDbContextFactory<VeloPortalDbContext> dbContextFactory,
          ILogger<ServReqInfRepository> logger,
          IConfiguration configuration
            )
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
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

        public async Task<IEnumerable<CompanyInf>?> GetCompanyInfoListByStatus(bool? is_active)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    var query = dbContext.CompanyInf.AsNoTracking().AsQueryable();


                    if (is_active.HasValue)
                        query = query.Where(p => p.is_active == is_active.Value);

                    return await query.ToListAsync();
                }

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Currency Retrival Failed");
                return null;
            }
        }

        public async Task<DtoUserInf?> FindUserByEmailOrPhoneAsync(string comcod, string user_type, string user_or_email)
        {

            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<DtoUserInf?>(null);
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod, "itv_portal.SP_USER_OPERATION", "Get_Portal_User_Info", user_type, user_or_email);

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

        public async Task<DtoCustomer?> FindUserByCustomerEmailAsync(string comcod, string user_type, string cust_email)
        {

            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<DtoCustomer?>(null);
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod, "itv_portal.SP_USER_OPERATION", "Get_CustomerProfile", user_type, cust_email);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<DtoCustomer?>(null);
                }

                var userList = ds.Tables[0].DataTableToList<DtoCustomer>();
                var user = userList?.FirstOrDefault();

                return await Task.FromResult(user);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<DtoCustomer?>(null);
            }
        }

        public async Task<bool> UpdatePasswordAsync(string comcod, string user_type, string userId, string new_password, string portal_role)
        {
            try
            {
                if (_spProcessAccess == null) return false;


                DataSet? ds = _spProcessAccess.GetTransInfo20( comcod, "itv_portal.SP_USER_OPERATION", "Update_Portal_User_Password", user_type, userId, new_password, portal_role);

                return ds != null;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return false;
            }
        }



        public async Task<long> InsertOrUpdateCustomer(DtoCustomer obj)
        {
            try
            {
               if (_spProcessAccess == null) return 0;


                DataSet? ds = _spProcessAccess.GetTransInfo20(obj.comcod, "itv_portal.SP_USER_OPERATION", "Update_Portal_CustomerProfile", "Customer",  obj.sup_user_id.ToString(), obj.fullname, obj.user_role, obj.suser_email, obj.suser_phone);

                return ds != null ? obj.sup_user_id : 0;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return 0;
            }
        }
    }
}
