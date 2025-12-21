using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using VeloPortal.Application.DTOs.ServiceRequest;
using VeloPortal.Application.Interfaces.FacilityManagement;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.FacilityManagement;
using VeloPortal.Domain.Enums;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.FacilityManagement
{
    public class ServReqInfRepository : IServReqInf
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public ServReqInfRepository(
          IDbContextFactory<VeloPortalDbContext> dbContextFactory,
          IConfiguration configuration)
        {
            _dbContextFactory = dbContextFactory;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<IEnumerable<DtoServiceRequest>?> GetUserConcernProjects(string? comcod, string user_role, string unq_id)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<IEnumerable<DtoServiceRequest>?>(null);
                }
                IEnumerable<DtoServiceRequest>? lst = null;
                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_portal.SP_USER_OPERATION", "Get_User_Concern_Projects", user_role, unq_id);
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<IEnumerable<DtoServiceRequest>?>(null);
                }

                lst = ds.Tables[0].DataTableToList<DtoServiceRequest>();
                return await Task.FromResult<IEnumerable<DtoServiceRequest>?>(lst);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<IEnumerable<DtoServiceRequest>?>(null);
            }
        }


        public async Task<ServReqInf?> InsertOrUpdateServReqInf(ServReqInf obj, string? action)
        {
            try
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    if (action == HelperEnums.Action.Add.ToString())
                    {
                        await dbContext.ServReqInf.AddAsync(obj);
                        await dbContext.SaveChangesAsync();

                        await dbContext.Entry(obj).ReloadAsync();
                        return obj;
                    }
                    else
                    {
                        ServReqInf? existloandata = await dbContext.ServReqInf.AsNoTracking().FirstOrDefaultAsync(p => p.service_req_id == obj.service_req_id);

                        if (existloandata != null)
                        {
                            dbContext.ServReqInf.Update(obj);
                            await dbContext.SaveChangesAsync();
                            return obj;
                        }
                        else
                        {
                            return null;
                        }
                    }
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
