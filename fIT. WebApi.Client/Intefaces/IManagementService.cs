using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fIT.WebApi.Client.Intefaces
{
    public interface IManagementService
    {
      Task<IManagementSession> LoginAsync(string upn, string password);
      Task UpdatePasswordAsync(string upn, string oldPassword, string newPassword);

      string EncryptString(String base64String);

    }
}
