using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using VeloPortal.Application.DTOs.Authentication;
using VeloPortal.Application.DTOs.Common;
using VeloPortal.Application.Interfaces.Authentication;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Enums;
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
          IConfiguration configuration)
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

        public async Task<VendorProfile?> FindUserByVendorEmailAsync(string comcod, string user_type, string vendor_email)
        {

            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<VendorProfile?>(null);
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod, "itv_portal.SP_USER_OPERATION", "Get_VendorProfile", user_type, vendor_email);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<VendorProfile?>(null);
                }

                var userList = ds.Tables[0].DataTableToList<VendorProfile>();
                var user = userList?.FirstOrDefault();

                return await Task.FromResult(user);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<VendorProfile?>(null);
            }
        }

        public async Task<SupportUser?> FindUserByCustomerEmailAsync(string comcod, string user_type, string cust_email)
        {

            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<SupportUser?>(null);
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod, "itv_portal.SP_USER_OPERATION", "Get_CustomerProfile", user_type, cust_email);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<SupportUser?>(null);
                }

                var userList = ds.Tables[0].DataTableToList<SupportUser>();
                var user = userList?.FirstOrDefault();

                return await Task.FromResult(user);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<SupportUser?>(null);
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


        public async Task<VendorProfile> InsertOrUpdateVendor(VendorProfile obj, string action)
        {
            try
            {

                using var dbContext = _dbContextFactory.CreateDbContext();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();


                if (action == HelperEnums.Action.Add.ToString())
                {

                    var lastVendorId = await dbContext.VendorProfile
                                        .OrderByDescending(v => v.vendorid)
                                        .Select(v => v.vendorid)
                                        .FirstOrDefaultAsync();

                    int nextIdNumber = 1;
                    if (!string.IsNullOrEmpty(lastVendorId))
                    {
                        if (int.TryParse(lastVendorId, out int lastId))
                        {
                            nextIdNumber = lastId + 1;
                        }
                    }

                    // "D10" formats the integer with leading zeros to 10 digits
                    obj.vendorid = nextIdNumber.ToString("D10");
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

                    var existingVendor = await dbContext.VendorProfile
                        .FirstOrDefaultAsync(v =>
                        v.comcod == obj.comcod &&
                        v.vendor_profile_id == obj.vendor_profile_id
                        );

              

                    if (existingVendor == null)
                    {
                        return null;
                    }


                    // Update properties
                    existingVendor.company_name = obj.company_name;
                    existingVendor.address = obj.address;
                    existingVendor.compan_overview = obj.compan_overview;
                    existingVendor.company_bin = obj.company_bin;
                    existingVendor.contact_no = obj.contact_no;
                    existingVendor.vendor_email = obj.vendor_email;
                    existingVendor.license_no = obj.license_no;
                    existingVendor.num_of_client = obj.num_of_client;
                    existingVendor.ong_num_of_client = obj.ong_num_of_client;
                    existingVendor.contact_person = obj.contact_person;
                    existingVendor.secondary_contact_no = obj.secondary_contact_no;
                    existingVendor.designation = obj.designation;
                    existingVendor.is_available = obj.is_available;
                    existingVendor.is_verify_acc = obj.is_verify_acc;
                    existingVendor.is_email_verify = obj.is_email_verify;
                    existingVendor.experience = obj.experience;
                    existingVendor.terms_condition = obj.terms_condition;
                    existingVendor.business_type = obj.business_type;
                    existingVendor.payment_mode = obj.payment_mode;
                    existingVendor.owner_name = obj.owner_name;
                    existingVendor.owner_id_no = obj.owner_id_no;
                    existingVendor.owner_tin_no = obj.owner_tin_no;
                    existingVendor.bankcode = obj.bankcode;
                    existingVendor.branch = obj.branch;
                    existingVendor.acc_name = obj.acc_name;
                    existingVendor.acc_number = obj.acc_number;
                    existingVendor.routeno = obj.routeno;
                    existingVendor.links = obj.links;
                    existingVendor.rescode = obj.rescode;
                    existingVendor.is_audit = obj.is_audit;
                    existingVendor.user_photo = obj.user_photo;
                    existingVendor.is_hold = obj.is_hold;
                    existingVendor.is_approved = obj.is_approved;

                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return obj;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                System.Diagnostics.Debug.WriteLine($"ERROR in InsertOrUpdateVendor: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return null;
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
