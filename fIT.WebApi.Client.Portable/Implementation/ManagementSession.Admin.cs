using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;

namespace fIT.WebApi.Client.Portable.Implementation
{
    public partial class ManagementSession : IAdminManagement
    {
        #region Field

        #endregion

        #region Ctor
        #endregion

        #region Methods
        /// <summary>
        /// Deletes a user this the given userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task DeleteUserAsync(string userId)
        {
            await this.DeleteAsync("/api/Accounts/User/{0}", userId);
        }

        /// <summary>
        /// Gets a user by its name
        /// </summary>
        /// <param name="username">username to search for</param>
        /// <returns></returns>
        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            return await this.GetAsync<UserModel>("/api/Accounts/User/{0}", username);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gives access to all admin-related actions
        /// </summary>
        public IAdminManagement Admins { get { return this; } }
        #endregion
    }
}
