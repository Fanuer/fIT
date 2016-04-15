using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels;
using fIT.App.Helpers;
using fIT.App.Interfaces;
using fIT.App.Pages;
using fIT.App.Repositories;
using fIT.App.Services;
using fIT.App.Utilities.Navigation;
using Xamarin.Forms;

namespace fIT.App
{
  public class App : Application
  {

    #region FIELDS
    #endregion

    #region CTOR
    public App()
    {
      AppName = "fIT.App";
      Settings.RefreshToken = "";
      IoCLocator.Current.RegisterServices(new Dictionary<Type, Type>() { { typeof(IRepository), typeof(Repository) } });
      IoCLocator.Current.RegisterViewModels(typeof(AppViewModelBase).Namespace);

      NavigationFrame frame = null;
      
      // Is Logged In
      if (String.IsNullOrEmpty(Settings.RefreshToken))
      {
        var vm = IoCLocator.Current.GetInstance<LoginViewModel>();
        frame = new NavigationFrame(vm);
      }
      else
      {
        var vm = IoCLocator.Current.GetInstance<ScheduleViewModel>();
        frame = new NavigationFrame(vm);
      }
      
      MainPage = frame.Root;
    }

    #endregion

    #region METHODS
    protected override void OnStart()
    {
      OnOffService.Current.Start();
      // Handle when your app starts
    }

    protected override void OnSleep()
    {
      OnOffService.Current.Stop();
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      OnOffService.Current.Start();
      // Handle when your app resumes
    }

    #endregion

    #region PROPERTIES
    public static string AppName { get; private set; }

    #endregion
  }
}
