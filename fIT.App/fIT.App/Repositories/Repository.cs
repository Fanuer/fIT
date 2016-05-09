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
    internal class Repository : IRepository
    {
        #region FIELDS

        private bool _status = true;
        private bool _isInitialised;
        private readonly object _lock = new object();
        #endregion

        #region CTOR

        public Repository()
        {
            OnOffService.Current.OnStatusChanged += OnConnectionStatusChanged;
        }
        #endregion

        #region METHODS

        public async Task<bool> LoginAsync(string username, string password)
        {
            var result = false;
            try
            {
                if (_status)
                {
                    var session = await ServerRespository.Current.Server.LoginAsync(username, password);
                    result = this.SetServerData(session, session.CurrentUserId);
                    if (!_isInitialised)
                    {
                        _isInitialised = !_isInitialised;
                    }
                }
                else
                {
                    result = await LocalRepository.Current.LoginAsync(username, password);
                }
                Debug.WriteLine("Login-Status: " + Environment.NewLine + "Status: " + (this._status ? "Online" : "Offline") + Environment.NewLine + "Successful: " + result);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error on login: " + e.Message + Environment.NewLine + e.StackTrace);
            }
            return result;
        }

        public async Task<IUserManagement> GetUserManagementAsync()
        {
            if (!_isInitialised)
            {
                await InitialiseAsync();
            }
            return _status ? ServerRespository.Current.ServerSession.Users : LocalRepository.Current.UserManagement;
        }

        public async Task<IAdminManagement> GetAdminManagementAsync()
        {
            if (!_isInitialised)
            {
                await InitialiseAsync();
            }
            return _status ? ServerRespository.Current.ServerSession.Admins : LocalRepository.Current.AdminManagement;
        }

        private async Task InitialiseAsync()
        {
            try
            {
                _status = await ServerRespository.Current.Server.PingAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            
            if (LoggedIn && _status)
            {
                var session = await ServerRespository.Current.Server.LoginAsync(Settings.RefreshToken);
                var user = await session.Users.GetUserDataAsync();
                SetServerData(session, new Guid(user.Id));
                _isInitialised = true;
            }
        }

        private bool SetServerData(IManagementSession session, Guid userId )
        {
            var result = false;

            try
            {
                if (session != null)
                {
                    lock (_lock)
                    {
                        ServerRespository.Current.ServerSession = session;
                        LocalRepository.Current.CurrentUserId = session.CurrentUserId;
                        Settings.RefreshToken = session.RefreshToken;
                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                Settings.RefreshToken = String.Empty;
                Debug.WriteLine($"Error on ServerLogin: {e.Message}");
            }
            return result;
        }

        private async void OnConnectionStatusChanged(object sender, ChangedOnlineStateEventArgs args)
        {
            this._status = args.Status;
            if (args.Status)
            {
                await LocalRepository.Current.SyncData();
                await InitialiseAsync();
            }
        }


        #endregion

        #region PROPERTIES

        public bool LoggedIn => !String.IsNullOrWhiteSpace(Settings.RefreshToken);

        #endregion
    }
}
