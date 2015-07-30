using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Account;

namespace fIT.WebApi.Client.Data.Intefaces
{
    public interface IUserManagement
    {
        /// <summary>
        /// Ändert das Nutzerpasswort
        /// </summary>
        /// <param name="model">password data</param>
        /// <returns></returns>
        Task UpdatePasswordAsync(ChangePasswordModel model);
        /// <summary>
        /// Ruft die Daten das aktuell angemeldeten Nutzers ab
        /// </summary>
        /// <returns></returns>
        Task<UserModel> GetUserDataAsync();
        /// <summary>
        /// Ändert die Daten des aktuell angemeldeten Nutzers
        /// </summary>
        /// <param name="newData"></param>
        /// <returns></returns>
        Task UpdateUserDataAsync(UserModel newData);


    }
}
