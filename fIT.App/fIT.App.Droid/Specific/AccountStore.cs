using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using fIT.App.Interfaces;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(fIT.App.Droid.Specific.AccountStore))]

namespace fIT.App.Droid.Specific
{
  public class AccountStore:IAccountStore
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
        Xamarin.Auth.AccountStore.Create(Forms.Context).Save(account, App.AppName);
      }
    }
    #endregion

    #region PROPERTIES
    #endregion
    public string Username => FitAccount?.Username;

    public string Password => FitAccount?.Properties["Password"];

    private Account FitAccount => Xamarin.Auth.AccountStore.Create(Forms.Context).FindAccountsForService(App.AppName).FirstOrDefault();
  }
}