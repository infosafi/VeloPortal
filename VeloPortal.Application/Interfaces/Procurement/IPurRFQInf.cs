using VeloPortal.Application.DTOs.Procurement;

namespace VeloPortal.Application.Interfaces.Procurement
{
    public interface IPurRFQInf
    {
        Task<string?> InsertorUpdateRequestForQuoteInfo(DtoRFQInf obj);
        Task<IEnumerable<dynamic>?> GetSingleRFQList(string? comcod, string? rfqid);
        Task<IEnumerable<DtoPeriodicRfqlist>?> GetPeriodicRfqlist(string? comcod, string? rescode);
    }
}
