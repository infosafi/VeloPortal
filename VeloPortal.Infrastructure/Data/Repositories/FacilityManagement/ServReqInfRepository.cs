using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ServReqInfRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;


        public ServReqInfRepository(
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

        public async Task<DtoServiceRequestDetails?> GetSingleServiceRequestDetailsInformation(string? comcod, long? service_req_id)
        {
            try
            {
                await Task.Delay(1);

                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return null;
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_fms.SP_FACILITY_MGT", "Get_Single_Service_Info", service_req_id.ToString() ?? "");

                if (ds == null || ds.Tables.Count < 3)
                {
                    _logger.LogWarning($"SP 'Get_Single_Service_Info' returned fewer than 3 tables for service_req_id: {service_req_id}");
                    return null;
                }

                IEnumerable<dynamic>? serviceInfo = ds.Tables[0].DataTableToDynamicList();
                IEnumerable<dynamic>? serviceProblemInfo = ds.Tables[1].DataTableToDynamicList();
                IEnumerable<dynamic>? serviceResourceInfo = ds.Tables[2].DataTableToDynamicList();
                IEnumerable<dynamic>? serviceTimelineInfo = ds.Tables[3].DataTableToDynamicList();
                IEnumerable<dynamic>? serviceExecutionTimeline = ds.Tables[4].DataTableToDynamicList();
                var resultDto = new DtoServiceRequestDetails
                {
                    ServiceInfo = serviceInfo,
                    ServiceProblemInfo = serviceProblemInfo,
                    ServiceResourceInfo = serviceResourceInfo,
                    ServiceTimelineInfo = serviceTimelineInfo,
                    serviceExecution = serviceExecutionTimeline
                };

                return resultDto;

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Single Service Details information Failed to Retrive");
                return null;
            }
        }

        public async Task<IEnumerable<DtoPortalUsersServiceRequest>?> GetPortalUsersServiceRequests(string? comcod, string user_role, string unq_id)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<IEnumerable<DtoPortalUsersServiceRequest>?>(null);
                }
                IEnumerable<DtoPortalUsersServiceRequest>? lst = null;
                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_portal.SP_USER_OPERATION", "Get_Portal_Users_Service_Requests", user_role, unq_id);
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<IEnumerable<DtoPortalUsersServiceRequest>?>(null);
                }

                lst = ds.Tables[0].DataTableToList<DtoPortalUsersServiceRequest>();
                return await Task.FromResult<IEnumerable<DtoPortalUsersServiceRequest>?>(lst);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<IEnumerable<DtoPortalUsersServiceRequest>?>(null);
            }
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
                using var dbContext = _dbContextFactory.CreateDbContext();
                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                if (action == HelperEnums.Action.Add.ToString())
                {
                    if (_spProcessAccess == null)
                    {
                        _logger.LogWarning("_spProcessAccess is not initialized.");
                        return null;
                    }

                    DataSet? ds = _spProcessAccess.GetTransInfo20(obj.comcod ?? "", "itv_fms.SP_FACILITY_MGT", "Get_Latest_Service_Code");

                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        return null;
                    }

                    var lst = ds.Tables[0].DataTableToDynamicList();
                    var latestServiceNo = lst.First().service_no.ToString();

                    if (string.IsNullOrWhiteSpace(latestServiceNo))
                    {
                        return null;
                    }

                    obj.service_no = latestServiceNo;
                    dbContext.ServReqInf.Add(obj);
                }
                else
                {
                    var exist = await dbContext.ServReqInf.FirstOrDefaultAsync(p =>
                        p.comcod == obj.comcod &&
                        p.service_req_id == obj.service_req_id &&
                        p.service_no == obj.service_no);

                    if (exist == null)
                    {
                        return null;
                    }

                    exist.customer_feedback = obj.customer_feedback;
                    exist.satisfaction = obj.satisfaction;
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return obj;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Service Request Info Information Insert or Update Failed");
                return null;
            }
        }

        public async Task<IEnumerable<DtoUserServiceRequestCounter>?> GetUserServiceReqeustCounter(string? comcod, string user_role, string unq_id)
        {
            try
            {
                if (_spProcessAccess == null)
                {
                    return await Task.FromResult<IEnumerable<DtoUserServiceRequestCounter>?>(null);
                }
                IEnumerable<DtoUserServiceRequestCounter>? lst = null;
                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_portal.SP_USER_OPERATION", "Get_Portal_Users_Service_Requests_Counter", user_role, unq_id);
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return await Task.FromResult<IEnumerable<DtoUserServiceRequestCounter>?>(null);
                }

                lst = ds.Tables[0].DataTableToList<DtoUserServiceRequestCounter>();
                return await Task.FromResult<IEnumerable<DtoUserServiceRequestCounter>?>(lst);
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                return await Task.FromResult<IEnumerable<DtoUserServiceRequestCounter>?>(null);
            }
        }

        public async Task<DtoPublicServiceRequestDetails?> GetSingleServiceRequestDetailsPublicInformation(string? comcod, long? service_req_id)
        {
            try
            {
                await Task.Delay(1);

                if (_spProcessAccess == null)
                {
                    _logger.LogWarning("_spProcessAccess is not initialized.");
                    return null;
                }

                DataSet? ds = _spProcessAccess.GetTransInfo20(comcod ?? "", "itv_fms.SP_FACILITY_MGT", "Get_Single_Service_Info", service_req_id.ToString() ?? "");

                if (ds == null || ds.Tables.Count < 3)
                {
                    _logger.LogWarning($"SP 'Get_Single_Service_Info' returned fewer than 3 tables for service_req_id: {service_req_id}");
                    return null;
                }

                IEnumerable<dynamic>? serviceInfo = ds.Tables[0].DataTableToDynamicList();
               
                var resultDto = new DtoPublicServiceRequestDetails
                {
                    ServiceInfo = serviceInfo 
                };

                return resultDto;

            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Single Service Details information Failed to Retrive");
                return null;
            }
        }
    }
}
