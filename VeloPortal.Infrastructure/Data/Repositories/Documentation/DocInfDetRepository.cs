using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VeloPortal.Application.Interfaces.Documentation;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Documentation;
using VeloPortal.Domain.Extensions;
using VeloPortal.Infrastructure.Data.DataContext;
using VeloPortal.Infrastructure.Data.SPHelper;

namespace VeloPortal.Infrastructure.Data.Repositories.Documentation
{
    public class DocInfDetRepository : IDocInfDet
    {
        private readonly IDbContextFactory<VeloPortalDbContext> _dbContextFactory;
        private readonly ILogger<DocInfDetRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly SPProcessAccess? _spProcessAccess;
        public DocInfDetRepository(
            IDbContextFactory<VeloPortalDbContext> dbContextFactory,
            ILogger<DocInfDetRepository> logger,
            IConfiguration configuration
            )
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString(DefaultSettings.DefaultDbconnection);
            _spProcessAccess = new SPProcessAccess(connectionString);
        }

        public async Task<bool> UploadDocument(List<DocInfDet> docInfDets)
        {
            try
            {
                if (docInfDets == null || !docInfDets.Any())
                {
                    _logger.LogWarning("Attempted to upload an empty or null document list.");
                    return false;
                }

                var invalidDocs = docInfDets.Where(d => string.IsNullOrEmpty(d.comcod) || string.IsNullOrEmpty(d.doc_url)).ToList();

                if (invalidDocs.Any())
                {
                    _logger.LogError("Document upload failed: {Count} documents are missing mandatory fields (comcod or doc_url).", invalidDocs.Count);
                    return false;
                }

                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    await dbContext.Set<DocInfDet>().AddRangeAsync(docInfDets);
                    var result = await dbContext.SaveChangesAsync();
                    // Return true if all records were successfully saved
                    return result == docInfDets.Count;
                }
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Document upload Failed during database insertion.");
                return false;
            }
        }

        public async Task<IEnumerable<dynamic>> GetFilteredDocumentsAsync(
         string comcod,
         DateTime? fromDate = null,
         DateTime? toDate = null,
         string? acccode = null,
         string? rescode = null,
         string? gencode = null,
         string? refno = null)
        {
            try
            {
                await Task.Delay(1);
                string desc1 = fromDate.HasValue ? fromDate.Value.ToString("dd-MMM-yyyy") : "";
                string desc2 = toDate.HasValue ? toDate.Value.ToString("dd-MMM-yyyy") : "";

                var ds = _spProcessAccess?.GetTransInfo20(
                    comCode: comcod,
                    SQLprocName: "itv_doc.SP_DOC_MGT",
                    CallType: "Get_Periodic_Filtered_Doc_List",
                    mDesc1: desc1,     // ← From Date
                    mDesc2: desc2,     // ← To Date
                    mDesc3: acccode ?? "",
                    mDesc4: rescode ?? "",
                    mDesc5: gencode ?? "",
                    mDesc6: refno ?? ""
                );

                if (ds?.Tables.Count is 0 || ds.Tables[0].Rows.Count == 0)
                    return Enumerable.Empty<dynamic>();


                return ds.Tables[0].DataTableToDynamicList();
                //var result = ds.Tables[0].AsEnumerable().Select(row =>
                //{
                //    dynamic item = new ExpandoObject();
                //    var dict = (IDictionary<string, object>)item;

                //    foreach (DataColumn col in ds.Tables[0].Columns)
                //    {
                //        var value = row.IsNull(col.ColumnName) ? null : row[col.ColumnName];
                //        dict[col.ColumnName] = value ?? "";
                //    }
                //    return item;
                //});

                //return result;
            }
            catch (Exception ex)
            {
                ErrorTrackingExtension.SetError(ex);
                _logger.LogError(ex, "Error fetching filtered documents - Comcod: {Comcod}", comcod);
                throw;
            }
        }

    }
}
