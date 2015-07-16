using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Client.Intefaces
{
  public interface IManagementSession: IDisposable
  {
    string Token { get; }
    DateTimeOffset ExpiresOn { get; }
    Task PerformRefreshAsync();

    IUserManagement Users { get; }
    IAdminManagement Admins { get; }
  }
}
