using fIT.WebApi.Client.Data.Intefaces;

namespace fIT.App.Interfaces
{
  public interface IRepository
  {
    IUserManagement UserManagement { get; }
    IAdminManagement AdminManagement { get; }

  }
}