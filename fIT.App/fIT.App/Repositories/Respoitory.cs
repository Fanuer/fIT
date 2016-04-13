using fIT.App.Data;
using fIT.App.Interfaces;
using fIT.App.Services;
using fIT.WebApi.Client.Data.Intefaces;

namespace fIT.App.Repositories
{
  internal class Respoitory:IRepository
  {
    #region FIELDS
    
    #endregion

    #region CTOR

    public Respoitory()
    {
      this.UserManagement = ServerRespository.Current.ServerSession.Users;
      this.AdminManagement = ServerRespository.Current.ServerSession.Admins;
      OnOffService.Current.OnStatusChanged += OnConnectionStatusChanged;
    }
    #endregion

    #region METHODS

    private async void OnConnectionStatusChanged(object sender, ChangedOnlineStateEventArgs args)
    {
      if (args.Status)
      {
        await LocalRepository.Current.SyncData();
        this.UserManagement = ServerRespository.Current.ServerSession.Users;
        this.AdminManagement = ServerRespository.Current.ServerSession.Admins;
      }
      else
      {
        this.UserManagement = LocalRepository.Current.UserManagement;
        this.AdminManagement = LocalRepository.Current.AdminManagement;
      }
    }
    #endregion

    #region PROPERTIES

    public IUserManagement UserManagement { get; private set; }
    public IAdminManagement AdminManagement { get; private set; }
    #endregion
  }
}
