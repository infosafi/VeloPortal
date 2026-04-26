namespace VeloPortal.Application.DTOs.AccountsFinance
{
    public class DtoFinCompReqResponse
    {
        public List<long> AffectedIds { get; set; } = new();
        public List<string> AffectedFcrNo { get; set; } = new();
        public string Detail { get; set; } = string.Empty;
    }
    public enum DtoFinCompReqAction
    {
        Save,
        Update
    }
    public class DtoFinCompReqRequest
    {
        public DtoFinCompReqAction Action { get; set; }

        public long? FinCompReqId { get; set; }

        public string Comcod { get; set; } = string.Empty;

        public string? Acccode { get; set; }

        public string? Rescode { get; set; }

        public string? Custcode { get; set; }

        public string? ReqSource { get; set; }

        public DateTime? ReqDate { get; set; }
        public string? Remarks { get; set; }
        public List<string>? ReqTypes { get; set; }
        public int? UserId { get; set; }
    }
    public class DtoFinCompReqDetails
    {
        public IEnumerable<dynamic> Data { get; set; } = Enumerable.Empty<dynamic>();
    }
}
