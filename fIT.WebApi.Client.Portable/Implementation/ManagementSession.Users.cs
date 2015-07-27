using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;

namespace fIT.WebApi.Client.Portable.Implementation
{
    public partial class ManagementSession : IUserManagement
    {
        #region Field
        #endregion

        #region Ctor
        #endregion

        #region Methods
        public async Task UpdatePasswordAsync(ChangePasswordModel model)
        {
            await PutAsJsonAsync(model, "/api/accounts/ChangePassword");
        }

        public async Task<UserModel> GetUserDataAsync()
        {
            return await GetAsync<UserModel>("api/Accounts/CurrentUser");
        }

        public async Task UpdateUserDataAsync(UserModel newData)
        {
            await PutAsJsonAsync(newData, "api/Accounts/CurrentUser");
        }

        #endregion

        #region Properties
        public IUserManagement Users { get { return this; } }

        #endregion

        
    }
}
