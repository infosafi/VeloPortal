using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using VeloPortal.Application.DTOs.ProjectImplementation;
using VeloPortal.Application.Interfaces.ProjectImplementation;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.ProjectImplementation;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.ProjectImplementation
{
    public class ContractorRepository : IContractorProfile
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;
        private readonly ILogger<ContractorRepository> _logger;


        public ContractorRepository(IDbContextFactory<VeloPortalDbContext> dbContextFactory, IConfiguration configuration, ILogger<ContractorRepository> logger)
        {
            _dbContextFactory = dbContextFactory;
            _configuration = configuration;
            _logger = logger;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<bool> IsContractorLoginAllowedAsync(string comcod, long contractorProfileId)
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                return await dbContext.ContractorProfile.AsNoTracking().AnyAsync(p => p.comcod == comcod && p.contractor_profile_id == contractorProfileId &&
                                                                                      p.is_email_verify == true && p.is_hold != true);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return false;
            }
        }


        public async Task<bool> SetEmailVerifiedAsync(string comcod, long contractorProfileId)
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                var v = await dbContext.ContractorProfile.FirstOrDefaultAsync(p => p.comcod == comcod && p.contractor_profile_id == contractorProfileId);

                if (v == null) return false;

                v.is_email_verify = true;

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return false;
            }
        }


        public async Task<ContractorProfile?> FindContractorByEmailAsync(string comcod, string email)
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                return await dbContext.ContractorProfile.AsNoTracking().FirstOrDefaultAsync(p => p.comcod == comcod && p.contractor_email == email);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return null;
            }
        }


        public async Task<ContractorProfile?> InsertOrUpdateContractor(ContractorProfile obj)
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                if (obj.contractor_profile_id == 0)
                {
                    var lastId = await dbContext.ContractorProfile.OrderByDescending(c => c.contractorid).Select(c => c.contractorid).FirstOrDefaultAsync();

                    obj.contractorid = (int.TryParse(lastId, out var n) ? n + 1 : 1).ToString("D10");

                    await dbContext.ContractorProfile.AddAsync(obj);
                }
                else
                {
                    var existing = await dbContext.ContractorProfile.FirstOrDefaultAsync(c => c.comcod == obj.comcod && c.contractor_profile_id == obj.contractor_profile_id)
                                        ?? throw new InvalidOperationException("Contractor not found.");


                    existing.company_name = obj.company_name;
                    existing.address = obj.address;
                    existing.compan_overview = obj.compan_overview;
                    existing.company_bin = obj.company_bin;
                    existing.contact_no = obj.contact_no;
                    existing.license_no = obj.license_no;
                    existing.num_of_client = obj.num_of_client;
                    existing.ong_num_of_client = obj.ong_num_of_client;
                    existing.contact_person = obj.contact_person;
                    existing.secondary_contact_no = obj.secondary_contact_no;
                    existing.designation = obj.designation;
                    existing.is_available = obj.is_available;
                    existing.experience = obj.experience;
                    existing.terms_condition = obj.terms_condition;
                    existing.business_type = obj.business_type;
                    existing.payment_mode = obj.payment_mode;
                    existing.owner_name = obj.owner_name;
                    existing.owner_id_no = obj.owner_id_no;
                    existing.owner_tin_no = obj.owner_tin_no;
                    existing.bankcode = obj.bankcode;
                    existing.branch = obj.branch;
                    existing.acc_name = obj.acc_name;
                    existing.acc_number = obj.acc_number;
                    existing.routeno = obj.routeno;
                    existing.links = obj.links;
                    existing.user_photo = obj.user_photo;

                    obj = existing;
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return obj;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex.InnerException ?? ex);
                _logger.LogError(ex, "Update failed");
                return null;
            }
        }

        public async Task<dynamic?> FindUserByContractorIdAsync(string comcod,string? fromdate, string? todate, string contractor_profile_id)
        {

            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<ContractorProfile?>(null);
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod, "itv_prj.SP_SUBCONTRACTING_MGT", "Get_Contractor_Info",fromdate ?? "", todate ?? "", contractor_profile_id ?? "");

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<ContractorProfile?>(null);
                }

                var userList = ds.Tables[0].DataTableToDynamicList();
                var user = userList?.FirstOrDefault();

                return await Task.FromResult(user);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<ContractorProfile?>(null);
            }
        }
        public async Task<IEnumerable<DtoContractorDashboardCounter>?> GetContractorDashboardCounter(string? comcod, string? user_role, string? user_id, string? res_code)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<IEnumerable<DtoContractorDashboardCounter>?>(null);
                }
                IEnumerable<DtoContractorDashboardCounter>? lst = null;
                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_prj.SP_CONTRACTOR_REPORTS", "Contractor_Dashboard_Counter", user_role ?? "", user_id ?? "", res_code ?? "");
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<IEnumerable<DtoContractorDashboardCounter>?>(null);
                }

                lst = ds.Tables[0].DataTableToList<DtoContractorDashboardCounter>();
                return await Task.FromResult<IEnumerable<DtoContractorDashboardCounter>?>(lst);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<IEnumerable<DtoContractorDashboardCounter>?>(null);
            }
        }

        public async Task<bool> SaveContractorSuply(IEnumerable<ContractorSuply> supplyItems)
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                foreach (var item in supplyItems)
                {
                    var existing = await dbContext.ContractorSuply
                        .FirstOrDefaultAsync(x => x.con_item_id == item.con_item_id && x.con_item_id != 0);

                    if (existing == null)
                    {
                        await dbContext.ContractorSuply.AddAsync(item);
                    }
                    else
                    {
                        dbContext.Entry(existing).CurrentValues.SetValues(item);
                    }
                }

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return false;
            }
        }

    }
}
