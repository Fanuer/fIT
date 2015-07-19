using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Client.Models.Account;

namespace fIT.WebApi.Client.Intefaces
{
    public interface IUserManagement
    {
        Task UpdatePasswordAsync(ChangePasswordModel model);

    }
}
