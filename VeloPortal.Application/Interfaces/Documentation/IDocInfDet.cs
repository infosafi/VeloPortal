using VeloPortal.Domain.Entities.Documentation;

namespace VeloPortal.Application.Interfaces.Documentation
{
    public interface IDocInfDet
    {
        Task<bool> UploadDocument(List<DocInfDet> docInfDets);
        Task<IEnumerable<dynamic>> GetFilteredDocumentsAsync(string? comcod, string? fromDate, string? toDate, string? acccode, string? rescode,string? gencode,string? refno);
    }
}
