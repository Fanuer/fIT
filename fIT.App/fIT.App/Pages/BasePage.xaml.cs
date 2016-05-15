using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels;
using fIT.App.Data.ViewModels.Abstract;
using fIT.App.Helpers;
using fIT.App.Services;
using Xamarin.Forms;

namespace fIT.App.Pages
{
    public partial class BasePage : ContentPage
    {
        #region FIELDS
        #endregion

        #region CTOR
        public BasePage()
        {
            InitializeComponent();
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

        #endregion

    }
}
