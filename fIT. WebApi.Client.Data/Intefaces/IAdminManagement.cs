using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.RefreshToken;
using fIT.WebApi.Client.Data.Models.Roles;

namespace fIT.WebApi.Client.Data.Intefaces
{
    public interface IAdminManagement
    {
        #region Users
        /// <summary>
        /// Löscht einen Nutzer
        /// </summary>
        /// <param name="userId">Id des Nutzers</param>
        /// <returns></returns>
        Task DeleteUserAsync(string userId);
        /// <summary>
        /// Ruft die Nutzerdaten fuer einen bestimmten Nutzer anhand seines Nutzernamens ab
        /// </summary>
        /// <param name="username">Name des Nutzers</param>
        /// <returns></returns>
        Task<UserModel> GetUserByUsernameAsync(string username);
        /// <summary>
        /// Ruft alle Nutzer ab
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserModel>> GetAllUsersAsync();
        /// <summary>
        /// Ruft die Nutzerdaten fuer einen bestimmten Nutzer anhand seiner Id ab
        /// </summary>
        /// <param name="id">ID des Nutzers</param>
        /// <returns></returns>
        Task<UserModel> GetUserByIdAsync(Guid id);

        #endregion

        #region Roles
        /// <summary>
        /// Ruft alle Rollen ab
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<RoleModel>> GetAllRolesAsync();
        /// <summary>
        /// Ruft anhand der Id eine Rolle
        /// </summary>
        /// <param name="id">Id der Rolle</param>
        /// <returns></returns>
        Task<RoleModel> GetRoleByIdAsync(string id);
        /// <summary>
        /// Ruft anhand des Names eine Rolle
        /// </summary>
        /// <param name="name">Name der Rolle</param>
        /// <returns></returns>
        Task<RoleModel> GetRoleByNameAsync(string name);
        /// <summary>
        /// Loescht eine Rolle
        /// </summary>
        /// <param name="id">Id der zu loeschenden Rolle</param>
        /// <returns></returns>
        Task DeleteRoleAsync(string id);
        /// <summary>
        /// Creates a new Role
        /// </summary>
        /// <param name="roleName">rolename</param>
        /// <returns></returns>
        Task<RoleModel> CreateRoleAsync(string roleName);
        /// <summary>
        /// Weist Nutzern Rollen zu
        /// </summary>
        /// <param name="model">Verbindung von Nutzern und Rollen</param>
        Task ManageUsersInRolesAsync(UsersInRoleModel model);

        #endregion

      #region Refreshtoken

        Task<IEnumerable<RefreshTokenModel>> GetAllRefreshtokensAsync();
        //Task DeleteRefreshtokenAsync(string tokenId);

      #endregion
    }
}
