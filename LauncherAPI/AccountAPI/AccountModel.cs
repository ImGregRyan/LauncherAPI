namespace AccountAPI
{
    public class AccountModel
    {
        public int? AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountPassword { get; set; }
        public string? AccountPasswordSalt { get; set; }
        public string? AccountPasswordKey { get; set; }
        public string? AccountEmail { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
