using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Account;

namespace fIT.WebApi.Client.Data.Intefaces
{
    public interface IUserManagement
    {
        Task UpdatePasswordAsync(ChangePasswordModel model);

    }
}
