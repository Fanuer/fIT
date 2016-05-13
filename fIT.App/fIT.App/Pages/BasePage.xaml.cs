﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels;
using fIT.App.Helpers;
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
            OnOffService.Current.OnStatusChanged += (sender, args) => OnlineStatus.Icon = args.Status ? OnlineIcon : OfflineIcon;
            this.BindingContextChanged += (sender, args) =>
            {
                if (!String.IsNullOrEmpty(Settings.RefreshToken))
                {
                    var vm = (AppViewModelBase) this.BindingContext;
                    if (vm != null)
                    {
                        this.ToolbarItems.Add(new ToolbarItem()
                        {
                            Icon = (FileImageSource) vm.Images.Logout.Source,
                            Text = "Logout",
                            Order = ToolbarItemOrder.Secondary,
                            Command = new Command(async () => await vm.LogoutAsync())
                        });
                    }
                }
            };
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
