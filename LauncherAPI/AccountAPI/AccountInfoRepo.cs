using Dapper;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace AccountAPI
{
    public class AccountInfoRepo : IAccountInfoRepo
    {
        private readonly DapperContext _context;

        public AccountInfoRepo(DapperContext context)
        {
            _context = context;
        }

        public async Task<List<AccountModel>> GetAllAccounts()
        {
            try
            {
                var query = @"exec spGetAllAccounts"; 

                using (var connection = _context.CreateConnection())
                {
                    var results = await connection.QueryAsync<AccountModel>(query);
                    return (List<AccountModel>)results;
                }
            }
            catch (Exception)
            {
                return new List<AccountModel>();
            }
        }

        public async Task<string> GetAccountSalt(AccountModel model)
        {
            try
            {
                //get salt from DB to compare for Login
                var query = @"exec spGetAccountSalt @AccountName";
                var values = new { AccountName = model.AccountName };

                using (var connection = _context.CreateConnection())
                {
                    var results = await connection.QueryAsync<AccountModel>(query, values);

                    var accountPasswordSalt = results.ToList()[0].AccountPasswordSalt;

                    return accountPasswordSalt;
                }
            }
            catch (Exception)
            {
                return "Could not find password salt.";
            }
        }

        public async Task<List<AccountModel>> LoginAccount(AccountModel model)
        {
            try
            {
                var salt = await GetAccountSalt(model);//Gets stored salt from specific account on db

                var byteSalt = Encoding.UTF8.GetBytes(salt);//converts to byte[] for hashing
                var bytePassword1 = Encoding.UTF8.GetBytes(model.AccountPassword);//converts to byte[] for hashing

                var hashed1 = GenerateSaltedHash(bytePassword1, byteSalt);//combines salt and password for complete hash

                var newPassword = Convert.ToBase64String(hashed1);//sends complete hashed pw, as a string, to DB for comparison to stored pw

                var query = @"exec spLoginAccount @AccountName, @AccountPassword";

                using (var connection = _context.CreateConnection())
                {
                    var results = await connection.QueryAsync<AccountModel>(query, new 
                    { 
                        AccountName = model.AccountName,
                        AccountPassword = newPassword
                    });
                    return (List<AccountModel>)results;
                }
            }
            catch (Exception)
            {
                return new List<AccountModel>();
            }
        }

        public async Task<int> RegisterAccount(AccountModel model)
        {
            var newSalt = Salt(5); // Generates a random salt
            var passwordKey = Salt(5); // Generate a random key so column is not empty

            var byteSalt = Encoding.UTF8.GetBytes(newSalt);//converts to byte[] for hashing
            var bytePassword1 = Encoding.UTF8.GetBytes(model.AccountPassword);//converts to byte[] for hashing

            var hashed1 = GenerateSaltedHash(bytePassword1, byteSalt);//combines salt and password1 to complete hash

            var newpassword = Convert.ToBase64String(hashed1);//set hashed password to a string to store on DB

            try
            {   
                var query = @"exec spRegisterAccount @AccountName, @AccountPassword, @AccountPasswordSalt, @AccountPasswordKey, @AccountEmail";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    { 
                        AccountName = model.AccountName,
                        AccountPassword = newpassword,
                        AccountPasswordSalt = newSalt,
                        AccountPasswordKey = passwordKey,
                        AccountEmail = model.AccountEmail                      
                    });
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }        
        }

        public async Task<int> SendPasswordKey(AccountModel model)
        {
            var passwordKey = Salt(5);
            try
            {
                var query = @"exec spSendPasswordKey @AccountName, @AccountPasswordKey, @AccountEmail";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    {
                        AccountName = model.AccountName,
                        AccountPasswordKey = passwordKey,
                        AccountEmail = model.AccountEmail
                    });
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> ResetPassword(AccountModel model)
        {
            var salt = await GetAccountSalt(model);//Gets stored salt from specific account on db

            var byteSalt = Encoding.UTF8.GetBytes(salt);//converts to byte[] for hashing
            var bytePassword1 = Encoding.UTF8.GetBytes(model.AccountPassword);//converts to byte[] for hashing

            var hashed1 = GenerateSaltedHash(bytePassword1, byteSalt);//combines salt and password for complete hash

            var newPassword = Convert.ToBase64String(hashed1);//sends complete hashed pw, as a string, to DB for comparison to stored pw

            try
            {   //Sends Hashed password, salt string to DB
                var query = @"exec spResetPassword @AccountName, @AccountPassword, @AccountPasswordKey, @AccountEmail";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    {
                        AccountName = model.AccountName,
                        AccountPassword = newPassword,
                        AccountPasswordKey = model.AccountPasswordKey,
                        AccountEmail = model.AccountEmail
                    });
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> ForgotAccountName(AccountModel model)
        {
            try
            {
                var query = @"exec spForgotAccountName @AccountEmail";

                using (var connection = _context.CreateConnection())
                {
                    var results = await connection.QueryAsync<int>(query, new
                    {
                        AccountEmail = model.AccountEmail,
                    });
                    return results.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }


        // Chat stuff
        public async Task<int> SendDatabaseWhisperMessage(ChatMessageModel model)
        {
            try
            {
                var query = @"exec spAddWhisperMessage @MessageType, @FromAccountName, @ToAccountName, @Message";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    {
                        MessageType = model.MessageType,
                        FromAccountName = model.FromAccountName,
                        ToAccountName = model.ToAccountName,
                        Message = model.Message,
                    });
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public async Task<List<ChatMessageModel>> GetChatHistory(ChatMessageModel model)
        {
            try
            {
                var query = @"exec spGetChatHistory @MessageType, @FromAccountName, @ToAccountName";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<ChatMessageModel>(query, new
                    {
                        MessageType = model.MessageType,
                        FromAccountName = model.FromAccountName,
                        ToAccountName = model.ToAccountName
                    });
                    return (List<ChatMessageModel>)response;
                }
            }
            catch (Exception)
            {
                return new List<ChatMessageModel>();
            }
        }

        // Requests stuff
        public async Task<int> SendRequest(RequestModel model)
        {
            try
            {
                var query = @"exec spSendRequest @RequestType, @FromAccountName, @ToAccountName";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    { 
                        RequestType = model.RequestType,
                        FromAccountName = model.FromAccountName,
                        ToAccountName = model.ToAccountName
                    });

                    var check = response.FirstOrDefault();
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<List<RequestModel>> GetAllRequests(RequestModel model)
        {
            try
            {
                var query = @"exec spGetAllRequests @FromAccountName";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<RequestModel>(query, new
                    { 
                        FromAccountName = model.FromAccountName
                    });

                    return (List<RequestModel>)response;
                }
            }
            catch (Exception)
            {
                return new List<RequestModel>();
            }
        }

        public async Task<int> CancelRequest(RequestModel model)
        {
            try
            {
                var query = @"exec spCancelRequest @RequestType, @FromAccountName, @ToAccountName";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    {
                        RequestType = model.RequestType,
                        FromAccountName = model.FromAccountName,
                        ToAccountName = model.ToAccountName
                    });

                    var check = response.FirstOrDefault();
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> AcceptRequest(RequestModel model)
        {
            try
            {
                var query = @"exec spAcceptRequest @RequestType, @FromAccountName, @ToAccountName";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    {
                        RequestType = model.RequestType,
                        FromAccountName = model.FromAccountName,
                        ToAccountName = model.ToAccountName
                    });

                    var check = response.FirstOrDefault();
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> DeclineRequest(RequestModel model)
        {
            try
            {
                var query = @"exec spDeclineRequest @RequestType, @FromAccountName, @ToAccountName";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    {
                        RequestType = model.RequestType,
                        FromAccountName = model.FromAccountName,
                        ToAccountName = model.ToAccountName
                    });

                    var check = response.FirstOrDefault();
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // Friends

        public async Task<List<FriendsOrIgnoredModel>> GetAllFriendsOrIgnored(FriendsOrIgnoredModel model)
        {
            try
            {
                var query = @"exec spGetAllFriendsOrIgnored @FirstAccountName";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<FriendsOrIgnoredModel>(query, new
                    {
                        FirstAccountName = model.FirstAccountName
                    });

                    return (List<FriendsOrIgnoredModel>)response;
                }
            }
            catch (Exception)
            {
                return new List<FriendsOrIgnoredModel>();
            }
        }

        public async Task<int> RemoveFriend(FriendsOrIgnoredModel model)
        {
            try
            {
                var query = @"exec spRemoveFriend @FirstAccountName, @SecondAccountName, @IsFriend";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    {
                        FirstAccountName = model.FirstAccountName,
                        SecondAccountName = model.SecondAccountName,
                        IsFriend = model.IsFriend
                    });

                    var check = response.FirstOrDefault();
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // Ignore
        public async Task<int> AddIgnore(FriendsOrIgnoredModel model)
        {
            try
            {
                var query = @"exec spAddIgnore @FirstAccountName, @SecondAccountName, @IsIgnored";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    {
                        FirstAccountName = model.FirstAccountName,
                        SecondAccountName = model.SecondAccountName,
                        IsIgnored = model.IsIgnored
                    });

                    var check = response.FirstOrDefault();
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> RemoveIgnore(FriendsOrIgnoredModel model)
        {
            try
            {
                var query = @"exec spRemoveIgnore @FirstAccountName, @SecondAccountName, @IsIgnored";

                using (var connection = _context.CreateConnection())
                {
                    var response = await connection.QueryAsync<int>(query, new
                    {
                        FirstAccountName = model.FirstAccountName,
                        SecondAccountName = model.SecondAccountName,
                        IsIgnored = model.IsIgnored
                    });

                    var check = response.FirstOrDefault();
                    return response.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }


        // Salt Generator
        private readonly Random _rng = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        private string Salt(int size)
        {
            char[] chars = new char[size];
            for(int i = 0; i < size; i++)
            {
                chars[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(chars);
        }
        static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }
            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
    }
}