using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using fIT.App.Helpers;
using fIT.App.Interfaces;
using fIT.App.Pages;
using fIT.App.Utilities.Navigation;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
  public class LoginViewModel:AppViewModelBase
  {
    #region FIELDS

    private string _username;
    private string _password;
    #endregion

    #region CTOR
    public LoginViewModel(IRepository rep) : base(rep)
    {
      Username = "";
      Password = "";
      Message = "";

      this.OnLoginClickedCommand = new Command(async ()=> { await OnLoginClicked(); });
      this.OnSignUpClickedCommand = new Command(async ()=> await this.ViewModelNavigation.PushAsync(IoCLocator.Current.GetInstance<ScheduleViewModel>()));
    }

    #endregion

    #region METHODS

    private async Task OnLoginClicked()
    {
      var result = await Repository.LoginAsync(this.Username, this.Password);
      if (result)
      {
        await this.ViewModelNavigation.ExchangeAync(IoCLocator.Current.GetInstance<ScheduleViewModel>());
      }
      else
      {
        this.Message = "Login failed";
        this.Password = "";
      }
    }

    #endregion

    #region PROPERTIES
    public ICommand OnLoginClickedCommand { get; private set; }

    public ICommand OnSignUpClickedCommand { get; private set; }

    public string Username
    {
      get
      {
        return _username;
      }
      set
      {
        Set(ref _username,  value);
      }
    }

    public string Password
    {
      get
      {
        return _password;
      }
      set
      {
        Set(ref _password, value);
      }
    }

    #endregion
  }
}
