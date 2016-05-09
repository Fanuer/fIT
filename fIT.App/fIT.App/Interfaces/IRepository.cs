using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;

namespace fIT.App.Interfaces
{
    public interface IRepository
    {
        Task<IUserManagement> GetUserManagementAsync();
        Task<IAdminManagement> GetAdminManagementAsync();
        bool LoggedIn { get; }

        Task<bool> LoginAsync(string username, string password);

    }
}