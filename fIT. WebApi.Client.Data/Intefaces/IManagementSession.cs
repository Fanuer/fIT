using System;
using System.Threading.Tasks;

namespace fIT.WebApi.Client.Data.Intefaces
{
  public interface IManagementSession: IDisposable
  {
    /// <summary>
    /// Encrypted Access Token
    /// </summary>
    string Token { get; }
    /// <summary>
    /// Access Token Expire Date
    /// </summary>
    DateTimeOffset ExpiresOn { get; }
    /// <summary>
    /// Performs a Refresh of the Access token
    /// </summary>
    /// <returns></returns>
    Task PerformRefreshAsync();

    /// <summary>
    /// All Methods a users with role 'User' can perform
    /// </summary>
    IUserManagement Users { get; }
    /// <summary>
    /// All Methods a users with role 'Admin' can perform
    /// </summary>
    IAdminManagement Admins { get; }
  }
}
