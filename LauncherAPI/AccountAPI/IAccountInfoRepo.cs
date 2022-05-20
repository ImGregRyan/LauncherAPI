
namespace AccountAPI
{
    public interface IAccountInfoRepo
    {
        public Task<List<AccountModel>> GetAllAccounts();
        public Task<string> GetAccountSalt(AccountModel model);
        public Task<List<AccountModel>> LoginAccount(AccountModel model);
        public Task<int> RegisterAccount(AccountModel model);
        public Task<int> ResetPassword(AccountModel model);
        public Task<int> SendPasswordKey(AccountModel model);
        public Task<int> ForgotAccountName(AccountModel model);
        // Chat
        public Task<int> SendDatabaseWhisperMessage(ChatMessageModel model);
        public Task<List<ChatMessageModel>> GetChatHistory(ChatMessageModel model);
        // Requests
        public Task<int> SendRequest(RequestModel model);
        public Task<List<RequestModel>> GetAllRequests(RequestModel model);
        public Task<int> CancelRequest(RequestModel model);
        public Task<int> AcceptRequest(RequestModel model);
        public Task<int> DeclineRequest(RequestModel model);
        // Friends
        public Task<List<FriendsOrIgnoredModel>> GetAllFriendsOrIgnored(FriendsOrIgnoredModel model);
        public Task<int> RemoveFriend(FriendsOrIgnoredModel model);
        // Ignore
        public Task<int> AddIgnore(FriendsOrIgnoredModel model);
        public Task<int> RemoveIgnore(FriendsOrIgnoredModel model);
    }
}