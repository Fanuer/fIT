using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels;
using fIT.App.Helpers;
using fIT.App.Utilities.Navigation.Attributes;
using Xamarin.Forms;

namespace fIT.App.Pages
{
  [RegisterViewModel(typeof(LoginViewModel))]
  public partial class LoginPage : BasePage
  {
    public LoginPage()
    {
      InitializeComponent();
    }
  }
}
