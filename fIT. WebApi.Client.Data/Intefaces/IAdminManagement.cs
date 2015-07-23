using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Account;

namespace fIT.WebApi.Client.Data.Intefaces
{
    public interface IAdminManagement
    {
        Task DeleteUserAsync(string userId);
        Task<UserModel> GetUserByUsernameAsync(string username);
    }
}
