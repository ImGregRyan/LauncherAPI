using Microsoft.AspNetCore.Mvc;

namespace AccountAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountApiController : ControllerBase
    {
        private readonly IAccountInfoRepo _accountInfoRepo;

        public AccountApiController(IAccountInfoRepo accountInfoRepo)
        {
            _accountInfoRepo = accountInfoRepo;
        }

        // Register and Login requests
        [HttpGet("GetAllAccounts")]
        public async Task<List<AccountModel>> GetAllAccounts()
        {
            return await _accountInfoRepo.GetAllAccounts();
        }

        [HttpPost("GetAccountSalt")]
        public async Task<string> GetAccountSalt(AccountModel model)
        {
            return await _accountInfoRepo.GetAccountSalt(model);
        }

        [HttpPost("LoginAccount")]
        public async Task<List<AccountModel>> LoginAccount(AccountModel model)
        {
            return await _accountInfoRepo.LoginAccount(model);
        }

        [HttpPost("RegisterAccount")]
        public async Task<int> RegisterAccount(AccountModel model)
        {
            return await _accountInfoRepo.RegisterAccount(model);
        }

        [HttpPost("ResetPassword")]
        public async Task<int> ResetPassword(AccountModel model)
        {
            return await _accountInfoRepo.ResetPassword(model);
        }

        [HttpPost("SendPasswordKey")]
        public async Task<int> SendPasswordKey(AccountModel model)
        {
            return await _accountInfoRepo.SendPasswordKey(model);
        }

        [HttpPost("ForgotAccountName")]
        public async Task<int> ForgotAccountName(AccountModel model)
        {
            return await _accountInfoRepo.ForgotAccountName(model);
        }


        // Chat Stuff

        [HttpPost("SendDatabaseWhisperMessage")]
        public async Task<int> SendDatabaseWhisperMessage(ChatMessageModel model)
        {
            return await _accountInfoRepo.SendDatabaseWhisperMessage(model);
        }

        [HttpPost("GetChatHistory")]
        public async Task<List<ChatMessageModel>> GetChatHistory(ChatMessageModel model)
        {
            return await _accountInfoRepo.GetChatHistory(model);
        }

        // Requests

        [HttpPost("SendRequest")]
        public async Task<int> SendRequest(RequestModel model)
        {
            return await _accountInfoRepo.SendRequest(model);
        }

        [HttpPost("GetAllRequests")]
        public async Task<List<RequestModel>> GetAllRequests(RequestModel model)
        {
            return await _accountInfoRepo.GetAllRequests(model);
        }

        [HttpPost("CancelRequest")]
        public async Task<int> CancelRequest(RequestModel model)
        {
            return await _accountInfoRepo.CancelRequest(model);
        }

        [HttpPost("AcceptRequest")]
        public async Task<int> AcceptRequest(RequestModel model)
        {
            return await _accountInfoRepo.AcceptRequest(model);
        }

        [HttpPost("DeclineRequest")]
        public async Task<int> DeclineRequest(RequestModel model)
        {
            return await _accountInfoRepo.DeclineRequest(model);
        }

        // Friends
        [HttpPost("GetAllFriendsOrIgnored")]
        public async Task<List<FriendsOrIgnoredModel>> GetAllFriendsOrIgnored(FriendsOrIgnoredModel model)
        {
            return await _accountInfoRepo.GetAllFriendsOrIgnored(model);
        }

        [HttpPost("RemoveFriend")]
        public async Task<int> RemoveFriend(FriendsOrIgnoredModel model)
        {
            return await _accountInfoRepo.RemoveFriend(model);
        }

        //Ignore
        [HttpPost("AddIgnore")]
        public async Task<int> AddIgnore(FriendsOrIgnoredModel model)
        {
            return await _accountInfoRepo.AddIgnore(model);
        }

        [HttpPost("RemoveIgnore")]
        public async Task<int> RemoveIgnore(FriendsOrIgnoredModel model)
        {
            return await _accountInfoRepo.RemoveIgnore(model);
        }
    }
}