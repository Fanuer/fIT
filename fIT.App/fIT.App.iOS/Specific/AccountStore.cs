using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AudioToolbox;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(fIT.App.iOS.Specific.AccountStore))]
namespace fIT.App.iOS.Specific
{
  public class AccountStore
  {
    #region FIELDS
    #endregion

    #region CTOR
    #endregion

    #region METHODS
    public void SaveCredentials(string userName, string password)
    {
      if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
      {
        Account account = new Account
        {
          Username = userName
        };
        account.Properties.Add("Password", password);
        Xamarin.Auth.AccountStore.Create().Save(account, App.AppName);
      }
    }
    #endregion

    #region PROPERTIES
    public string Username => FitAccount?.Username;

    public string Password => FitAccount?.Properties["Password"];

    private Account FitAccount => Xamarin.Auth.AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();

    #endregion
  }
}
