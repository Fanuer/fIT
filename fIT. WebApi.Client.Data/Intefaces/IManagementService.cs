using System;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Account;

namespace fIT.WebApi.Client.Data.Intefaces
{
    public interface IManagementService
    {
        Task<IManagementSession> LoginAsync(string username, string password);
        Task<bool> PingAsync();
        Task RegisterAsync(CreateUserModel model);
    }
}
