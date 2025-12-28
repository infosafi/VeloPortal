using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VeloPortal.Application.Interfaces.Documentation;
using VeloPortal.Application.Settings;
using VeloPortal.Domain.Entities.Documentation;
using VeloPortal.Domain.Extensions;

namespace VeloPortal.WebApi.Controllers.V1.Documentation
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class DocumentationController(IDocInfDet _docInfDet) : ControllerBase
    {
        #region Upload Documentation
        /// <summary>
        /// List Documentation Upload
        /// </summary>
        /// The list of <see cref="DocInfDet"/> objects to be deleted.
        [HttpPost("upload-docs")]
        public async Task<IActionResult> UploadDocument(List<DocInfDet> docInfDets)
        {
            if (docInfDets == null)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(
                 new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "List of Document Should Not Null or Empty"));
            }

            var response = await _docInfDet.UploadDocument(docInfDets);

            if (response)
            {
                return Ok(ApiResponse<bool>.SuccessResponse(response, message: "Document Upload successfully"));
            }
            return BadRequest(ApiResponse<bool>.FailureResponse(new List<string> { ErrorTrackingExtension.ErrorMsg ?? "Error Occured" }, "Document upload Failed"));
        }
        #endregion
        #region Get Filtered Document List
        /// <summary>
        /// Retrieve filtered document list with date range and code filters
        /// </summary>
        /// <param name="comcod">Company Code (Required)</param>
        /// <param name="fromDate">From upload date (optional) → Format: YYYY-MM-DD</param>
        /// <param name="toDate">To upload date (optional) → Format: YYYY-MM-DD</param>
        /// <param name="acccode">Project/Account Code (optional - partial match)</param>
        /// <param name="rescode">Resource Code (optional - partial match)</param>
        /// <param name="gencode">General Code (optional - partial match)</param>
        /// <param name="refno">Reference No (optional - partial match)</param>
        /// <returns>Filtered documents with full descriptions</returns>
        [HttpGet("filtered-docs")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<dynamic>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<IActionResult> GetFilteredDocuments(
            [FromQuery] string comcod,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? acccode = null,
            [FromQuery] string? rescode = null,
            [FromQuery] string? gencode = null,
            [FromQuery] string? refno = null)
        {
            if (string.IsNullOrWhiteSpace(comcod))
                return BadRequest(ApiResponse<string>.FailureResponse("comcod is required"));

            var documents = await _docInfDet.GetFilteredDocumentsAsync(
                comcod, fromDate, toDate, acccode, rescode, gencode, refno);

            var message = documents.Any()
                ? "Documents retrieved successfully"
                : "No documents found for the given filters";

            return Ok(ApiResponse<IEnumerable<dynamic>>.SuccessResponse(
                data: documents,
                message: message
            ));
        }
        #endregion


    }
}
