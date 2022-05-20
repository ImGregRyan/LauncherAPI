namespace AccountAPI
{
    public class FriendsOrIgnoredModel
    {
        public int? FriendshipId { get; set; }
        public int? FirstAccountId { get; set; }
        public int? SecondAccountId { get; set; }
        public string? FirstAccountName { get; set; }
        public string? SecondAccountName { get; set; }
        public bool? IsFriend { get; set; }
        public bool? IsIgnored { get; set; }
    }
}
