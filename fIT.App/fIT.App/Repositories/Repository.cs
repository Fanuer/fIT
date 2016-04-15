using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using fIT.App.Data;
using fIT.App.Helpers;
using fIT.App.Interfaces;
using fIT.App.Services;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;

namespace fIT.App.Repositories
{
  internal class Repository:IRepository
  {
    #region FIELDS

    private bool _status;
    #endregion

    #region CTOR

    public Repository()
    {
      this.UserManagement = ServerRespository.Current?.ServerSession?.Users;
      this.AdminManagement = ServerRespository.Current?.ServerSession?.Admins;
      OnOffService.Current.OnStatusChanged += OnConnectionStatusChanged;
    }
    #endregion

    #region METHODS

    private async void OnConnectionStatusChanged(object sender, ChangedOnlineStateEventArgs args)
    {
      this._status = args.Status;
      if (args.Status)
      {
        await LocalRepository.Current.SyncData();
        this.UserManagement = ServerRespository.Current.ServerSession?.Users;
        this.AdminManagement = ServerRespository.Current.ServerSession?.Admins;
      }
      else
      {
        this.UserManagement = LocalRepository.Current.UserManagement;
        this.AdminManagement = LocalRepository.Current.AdminManagement;
      }
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
      var result = false;
      try
      {
        if (_status)
        {
          var session = await ServerRespository.Current.Server.LoginAsync(username, password);
          if (session != null)
          {
            this.UserManagement = session.Users;
            this.AdminManagement = session.Admins;
            LocalRepository.Current.CurrentUserId = session.CurrentUserId;
            Settings.RefreshToken = session.RefreshToken;
            result = true;
          }
        }
        else
        {
          result = await LocalRepository.Current.LoginAsync(username, password);
          this.UserManagement = LocalRepository.Current.UserManagement;
          this.AdminManagement = LocalRepository.Current.AdminManagement;
        }
        Debug.WriteLine("Login-Status: " + Environment.NewLine + "Status: " + (this._status?"Online":"Offline")+ Environment.NewLine + "Successful: " + result);
      }
      catch (Exception e)
      {
        Debug.WriteLine("Error on login: " + e.Message + Environment.NewLine + e.StackTrace);
      }
      return result;
    }

    #endregion

    #region PROPERTIES

    public IUserManagement UserManagement { get; private set; }
    public IAdminManagement AdminManagement { get; private set; }

    public bool LoggedIn => _status ? this.UserManagement != null : true;
    
    #endregion
  }
}
