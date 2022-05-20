namespace AccountAPI
{
    public class RequestModel
    {
        public int? RequestId { get; set; }
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
        public string? FromAccountName { get; set; }
        public string? ToAccountName { get; set; }
        public string? RequestType { get; set; }
        public string? ClanName { get; set; }
        public string? PartyName { get; set; }
    }
}
