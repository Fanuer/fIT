using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.App.Interfaces
{
  public interface IAccountStore
  {
    string Username { get; }
    string Password { get; }

    void SaveCredentials(string userName, string password);
  }
}
