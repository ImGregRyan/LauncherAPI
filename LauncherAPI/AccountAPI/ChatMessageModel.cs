namespace AccountAPI
{
    public class ChatMessageModel
    {
        public int? MessageId { get; set; }
        public string? MessageType { get; set; }
        public string? FromAccountName { get; set; }
        public string? ToAccountName { get; set; }
        public string? Message { get; set; }
        public DateTime? MessageTime { get; set; }
        public int? ChatGroupId { get; set; }
    }
}
