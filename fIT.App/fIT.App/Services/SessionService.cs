using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Portable.Implementation;

namespace fIT.App.Services
{
  internal class SessionService
  {
    #region Const
    private const string URL = @"http://fit-bachelor.azurewebsites.net/";
    
    #endregion

    #region FIELDS
    private static SessionService _current;

    #endregion

    #region CTOR

    private SessionService()
    {
      this.Server = new ManagementService(SessionService.URL);
    }
    #endregion

    #region METHODS
    public async Task RenewSessionAsync(LoginModel model)
    {
      if (model != null)
      {
        this.ServerSession = await this.Server.LoginAsync(model.Username, model.Password);
      }
    }
    #endregion

    #region PROPERTIES

    public static SessionService Current 
    {
      get
      {
        if (SessionService._current != null)
        {
          SessionService._current = new SessionService();
        }
        return SessionService._current;
      }
    }

    public IManagementSession ServerSession { get; private set; }
    public IManagementService Server { get; private set; }
    #endregion
  }
}
