using VeloPortal.Application.DTOs.AccountsFinance;
using VeloPortal.Application.Settings;

namespace VeloPortal.Application.Interfaces.AccountsFinance
{
    public interface IFinCompReq
    {
        Task<string> GenerateFcrNoAsync(string comcod, DateTime reqDate);
        Task<ApiResponse<DtoFinCompReqResponse>> SaveAsync(DtoFinCompReqRequest request);
        Task<ApiResponse<DtoFinCompReqResponse>> UpdateAsync(DtoFinCompReqRequest request);
        Task<ApiResponse<DtoFinCompReqDetails>> GetFinCompDetailsAsync(string comcod, long? finCompReqId = null, string? fcrno = null, string? acccode = null, string? rescode = null, string? custcode = null, string? reqtype = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
