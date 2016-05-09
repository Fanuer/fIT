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
    #region FIELDS
    private const string _offlineIcon = "Check.png";
    private const string _onlineIcon = "DoubleCheck.png";
    #endregion

    #region CTOR
    public BasePage()
    {
      InitializeComponent();
      /*OnlineStatus = new ToolbarItem
      {
        Text = "OnlineStatus",
        Order = ToolbarItemOrder.Primary,
        Icon = this.OnlineIcon
      };
      this.ToolbarItems.Add(OnlineStatus);*/
      OnOffService.Current.OnStatusChanged += (sender, args) => OnlineStatus.Icon = args.Status ? OnlineIcon : OfflineIcon;
    }
    #endregion

    #region METHODS
    #endregion

    #region PROPERTIES

    private ToolbarItem OnlineStatus { get; set; }

    private string OnlineIcon => String.Concat(Device.OnPlatform("Icons/", "", "Assets/Icons"), _onlineIcon);
    private string OfflineIcon => String.Concat(Device.OnPlatform("Icons/", "", "Assets/Icons"), _offlineIcon);

    #endregion

  }
}
