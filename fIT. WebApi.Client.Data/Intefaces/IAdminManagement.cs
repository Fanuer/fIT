using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Account;

namespace fIT.WebApi.Client.Data.Intefaces
{
    public interface IAdminManagement
    {
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
    }
}
