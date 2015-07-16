using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fIT.WebApi.Client.Intefaces
{
    public interface IManagementService
    {
      Task<IManagementSession> LoginAsync(string username, string password);
      Task UpdatePasswordAsync(string username, string oldPassword, string newPassword);

      string EncryptString(String base64String);

    }
}
