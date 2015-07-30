using System.Threading.Tasks;
using fITNat.ApiAccess.Intefaces;
using fITNat.ApiAccess.Models.Account;

namespace fITNat.ApiAccess.Implementation
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
        #endregion

        #region Properties
        public IUserManagement Users { get { return this; } }

        #endregion

        
    }
}
