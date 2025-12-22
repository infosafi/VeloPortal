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
    }
}
