using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Services;

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
      // The root page of your application
      MainPage = new ContentPage
      {
        Content = new StackLayout
        {
          VerticalOptions = LayoutOptions.Center,
          Children = {
            new Label {
              XAlign = TextAlignment.Center,
              Text = "Welcome to Xamarin Forms!"
            }
          }
        }
      };
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
