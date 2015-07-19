using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Client.Intefaces;
using fIT.WebApi.Client.Models.Account;

namespace fIT.WebApi.Client.Implementation
{
    public partial class ManagementSession : IAdminManagement
    {
        #region Field

        #endregion

        #region Ctor
        #endregion

        #region Methods
        public async Task DeleteUserAsync(string userId)
        {
            await this.DeleteAsync("/api/Accounts/User/{0}", userId);
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            return await this.GetAsync<UserModel>("/api/Accounts/User/{0}", username);
        }
        #endregion

        #region Properties
        public IAdminManagement Admins { get { return this; } }
        #endregion
    }
}
