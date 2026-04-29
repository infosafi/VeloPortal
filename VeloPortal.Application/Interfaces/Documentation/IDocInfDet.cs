using VeloPortal.Domain.Entities.Documentation;

namespace VeloPortal.Application.Interfaces.Documentation
{
    public interface IDocInfDet
    {
        Task<bool> UploadDocument(List<DocInfDet> docInfDets);
        Task<IEnumerable<dynamic>> GetFilteredDocumentsAsync(
          string comcod,
          DateTime? fromDate = null,
          DateTime? toDate = null,
          string? acccode = null,
          string? rescode = null,
          string? gencode = null,
           string? refno = null);
    }
}
