using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Services;
using Xamarin.Forms;

namespace fIT.App.Pages
{
  public partial class BasePage : ContentPage
  {
    public BasePage()
    {
      InitializeComponent();
      OnlineStatus = new ToolbarItem
      {
        Text = "OnlineStatus",
        Order = ToolbarItemOrder.Primary,
        Icon = this.OfflineIcon
      };
      this.ToolbarItems.Add(OnlineStatus);
      OnOffService.Current.OnStatusChanged += (sender, args) => OnlineStatus.Icon = args.Status ? OnlineIcon : OfflineIcon;
    }


    protected ToolbarItem OnlineStatus { get; set; }

    private string OfflineIcon => "Check.png";
    private string OnlineIcon => "DoubleCheck.png";
  }
}
