using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Client.Models.Account;

namespace fIT.WebApi.Client.Intefaces
{
    public interface IAdminManagement
    {
        Task DeleteUserAsync(string userId);
        Task<UserModel> GetUserByUsernameAsync(string username);
    }
}
