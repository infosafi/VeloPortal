using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeloPortal.Application.Interfaces.ProjectImplementation;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Authentication;
using VeloPortal.Domain.Entities.ProjectImplementation;
using VeloPortal.Domain.Enums;
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


        public ContractorRepository(
          IDbContextFactory<VeloPortalDbContext> dbContextFactory,
          IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }
        public async Task<ContractorProfile> InsertOrUpdateContractor(ContractorProfile obj)
        {
            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                if (obj.contractor_profile_id == 0)
                {
                    var lastContractorId = await dbContext.ContractorProfile
                        .OrderByDescending(c => c.contractorid)
                        .Select(c => c.contractorid)
                        .FirstOrDefaultAsync();

                    int nextIdNumber = 1;
                    if (!string.IsNullOrEmpty(lastContractorId) && int.TryParse(lastContractorId, out int lastId))
                    {
                        nextIdNumber = lastId + 1;
                    }

                    // "D10" formats the integer with leading zeros to 10 digits
                    obj.contractorid = nextIdNumber.ToString("D10");
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

                    await dbContext.ContractorProfile.AddAsync(obj);
                }
                else
                {
                    var existingContractor = await dbContext.ContractorProfile
                        .FirstOrDefaultAsync(c =>
                            c.comcod == obj.comcod &&
                            c.contractor_profile_id == obj.contractor_profile_id);

                    if (existingContractor == null)
                    {
                        return null;
                    }

                    existingContractor.company_name = obj.company_name;
                    existingContractor.address = obj.address;
                    existingContractor.compan_overview = obj.compan_overview;
                    existingContractor.company_bin = obj.company_bin;
                    existingContractor.contact_no = obj.contact_no;
                    // NOTE: email, status flags and rescode are intentionally NOT updatable here.
                    existingContractor.license_no = obj.license_no;
                    existingContractor.num_of_client = obj.num_of_client;
                    existingContractor.ong_num_of_client = obj.ong_num_of_client;
                    existingContractor.contact_person = obj.contact_person;
                    existingContractor.secondary_contact_no = obj.secondary_contact_no;
                    existingContractor.designation = obj.designation;
                    existingContractor.is_available = obj.is_available;
                    existingContractor.experience = obj.experience;
                    existingContractor.terms_condition = obj.terms_condition;
                    existingContractor.business_type = obj.business_type;
                    existingContractor.payment_mode = obj.payment_mode;
                    existingContractor.owner_name = obj.owner_name;
                    existingContractor.owner_id_no = obj.owner_id_no;
                    existingContractor.owner_tin_no = obj.owner_tin_no;
                    existingContractor.bankcode = obj.bankcode;
                    existingContractor.branch = obj.branch;
                    existingContractor.acc_name = obj.acc_name;
                    existingContractor.acc_number = obj.acc_number;
                    existingContractor.routeno = obj.routeno;
                    existingContractor.links = obj.links;
                    existingContractor.user_photo = obj.user_photo;

                    obj = existingContractor;
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return obj;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                System.Diagnostics.Debug.WriteLine($"ERROR in InsertOrUpdateContractor: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return null;
            }
        }


    }
}
