using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.RefreshToken;
using fIT.WebApi.Client.Data.Models.Roles;

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

        /// <summary>
        /// Returns all users
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            return await this.GetAsync<IEnumerable<UserModel>>("/api/Accounts/User");
        }

        /// <summary>
        /// Gets a user by its id
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        public async Task<UserModel> GetUserByIdAsync(Guid id)
        {
            return await this.GetAsync<UserModel>("/api/Accounts/User/{0}", id);
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        public async Task<IEnumerable<RoleModel>> GetAllRolesAsync()
        {
            return await GetAsync<IEnumerable<RoleModel>>("/api/Roles");
        }

        /// <summary>
        /// Gets a roles data by its id
        /// </summary>
        /// <param name="id">role id</param>
        /// <returns></returns>
        public async Task<RoleModel> GetRoleByIdAsync(string id)
        {
            return await GetAsync<RoleModel>("/api/Roles/" + id);
        }

        /// <summary>
        /// Gets a roles data by its name
        /// </summary>
        /// <param name="name">role name</param>
        public async Task<RoleModel> GetRoleByNameAsync(string name)
        {
          return await GetAsync<RoleModel>("/api/Roles/" + name);
        }

        /// <summary>
        /// Deletes an existing role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteRoleAsync(string id)
        {
            await DeleteAsync("/api/Roles/" + id);
        }

        /// <summary>
        /// Creates a new Role
        /// </summary>
        /// <param name="roleName">Name of the new role. Must be unique</param>
        /// <returns></returns>
        public async Task<RoleModel> CreateRoleAsync(string roleName)
        {
            return await PostAsJsonReturnAsync<object, RoleModel>(new {Name = roleName}, "/api/Roles");
        }

        /// <summary>
        /// Change the relationships between users and roles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ManageUsersInRolesAsync(UsersInRoleModel model)
        {
            await PutAsJsonAsync(model, "/api/Roles/ManageUsersInRole");
        }

      public async Task<IEnumerable<RefreshTokenModel>> GetAllRefreshtokensAsync()
      {
        return await this.GetAsync<IEnumerable<RefreshTokenModel>>("/api/RefreshTokens");
      }

      public async Task DeleteRefreshtokenAsync(string tokenId)
      {
        await DeleteAsJsonAsync(new {tokenId}, "/api/RefreshTokens");
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
