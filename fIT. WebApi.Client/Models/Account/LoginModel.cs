using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Client.Models.Account
{
  public class LoginModel
  {
    public string Username { get; set; }
    public string Password { get; set; }
    public string Grant_Type { get; set; }
  }
}
