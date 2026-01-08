using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using VeloPortal.Application.DTOs.Common;
using VeloPortal.Application.Interfaces.Authentication;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Enums;
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


        public async Task<int> InsertOrUpdateVendor(VendorProfile obj, string action)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    if (action == HelperEnums.Action.Add.ToString())
                    {
       
                        obj.experience ??= 0;     
                        obj.business_type ??= 0; 
                        obj.num_of_client ??= 0;   
                        obj.ong_num_of_client ??= 0; 

                        obj.company_bin ??= "";
                        obj.compan_overview ??= "";
                        obj.acc_name ??= "";
                        obj.acc_number ??= "";
                        obj.address ??= "";
                        obj.bankcode ??= "";
                        obj.branch ??= "";
                        obj.routeno ??= "";
                        obj.designation ??= "";
                        obj.contact_person ??= "";
                        obj.secondary_contact_no ??= "";
                        obj.owner_name ??= "";
                        obj.owner_id_no ??= "";
                        obj.owner_tin_no ??= "";
                        obj.rescode ??= "";
                        obj.license_no ??= "";
                        obj.terms_condition ??= "";
                        obj.payment_mode ??= "";
                        obj.links ??= "";
                        obj.user_photo ??= "";

                        await dbContext.VendorProfile.AddAsync(obj);
                    }
                    else
                    {
                        dbContext.VendorProfile.Update(obj);
                    }

                    await dbContext.SaveChangesAsync();
                    return obj.vendor_profile_id;
                }
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return 0;
            }
        }
    }
}
