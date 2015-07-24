using System;
using System.Threading.Tasks;
using fIT.WebApi.Client.Models.Account;

namespace fIT.WebApi.Client.Intefaces
{
    public interface IManagementService
    {
        Task<IManagementSession> LoginAsync(string username, string password);
        Task<bool> PingAsync();
        Task RegisterAsync(CreateUserModel model);

        string EncryptString(String base64String);

    }
}
