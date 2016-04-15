using System;
using System.Threading.Tasks;
using fIT.App.Helpers;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Portable.Implementation;

namespace fIT.App.Repositories
{
  internal class ServerRespository
  {
    #region Const
    private const string URL = @"http://fit-bachelor.azurewebsites.net/";
    
    #endregion

    #region FIELDS
    private static ServerRespository _current;

    #endregion

    #region CTOR

    private ServerRespository()
    {
      this.Server = new ManagementService(ServerRespository.URL);
      Task.Run(TryGetSessionAsync);
    }
    #endregion

    #region METHODS

    private async Task TryGetSessionAsync()
    {
      if (!String.IsNullOrWhiteSpace(Settings.RefreshToken))
      {
        this.ServerSession = await this.Server.LoginAsync(Settings.RefreshToken);
      }
    }
    #endregion

    #region PROPERTIES

    public static ServerRespository Current 
    {
      get
      {
        if (ServerRespository._current == null)
        {
          ServerRespository._current = new ServerRespository();
        }
        return ServerRespository._current;
      }
    }

    public IManagementSession ServerSession { get; private set; }
    public IManagementService Server { get; private set; }
    #endregion
  }
}
