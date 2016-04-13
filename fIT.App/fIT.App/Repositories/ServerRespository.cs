using System.Threading.Tasks;
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

    public static ServerRespository Current 
    {
      get
      {
        if (ServerRespository._current != null)
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
