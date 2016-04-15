using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;

namespace fIT.App.Interfaces
{
  public interface IRepository
  {
    IUserManagement UserManagement { get; }
    IAdminManagement AdminManagement { get; }
    bool LoggedIn { get; }

    Task<bool> LoginAsync(string username, string password);

  }
}