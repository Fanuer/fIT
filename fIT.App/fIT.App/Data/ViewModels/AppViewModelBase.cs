using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using AutoMapper;
using fIT.App.Data.Datamodels;
using fIT.App.Helpers;
using fIT.App.Interfaces;
using fIT.App.Utilities.Navigation;
using fIT.App.Utilities.Navigation.Interfaces;
using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public abstract class AppViewModelBase : ViewModelBase, INavigatingViewModel
    {
        #region FIELDS

        private string _message;
        private string _title;
        private bool _isLoading;
        #endregion

        #region CTOR

        protected AppViewModelBase(IUserDialogs userDialogs, string title)
        {
            
            this.Title = title;
            this.Colors = new ColorViewModel();
            this.FitLogo = ImageSource.FromResource("fIT.App.Resources.Images.Icon.png");
            this.UserDialogs = userDialogs;

            Task.Run(InitAsync);
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(IsLoading)))
                {
                    if (IsLoading)
                    {
                        this.UserDialogs.ShowLoading();
                    }
                    else
                    {
                        this.UserDialogs.HideLoading();
                    }
                }
            };
        }
        
        #endregion

        #region METHODS

        protected virtual Task InitAsync()
        {
            return null;
        }

        public async Task LogoutAsync()
        {
            Settings.RefreshToken = "";
            await this.ViewModelNavigation.PopToRootAsync();
            await this.ViewModelNavigation.ExchangeAync(IoCLocator.Current.GetInstance<LoginViewModel>());
        }
        #endregion

        #region PROPERTIES

        protected IRepository Repository => IoCLocator.Current.GetInstance<IRepository>();
        protected IUserDialogs UserDialogs { get; private set; }
        protected IMapper AutoMapper => IoCLocator.Current.GetInstance<IMapper>();
        /// <summary>
        /// Message that provides feedback to a user
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { this.Set(ref this._message, value); }
        }

        /// <summary>
        /// Page Title
        /// </summary>
        public string Title
        {
            get { return _title; }
            private set { this.Set(ref this._title, value); }
        }

        /// <summary>
        /// Navigation
        /// </summary>
        public IViewModelNavigation ViewModelNavigation { get; set; }

        public ColorViewModel Colors { get; private set; }

        public ImageSource FitLogo{ get; private set; }

        /// <summary>
        /// Shows, if a Page is waiting for an action to end
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.Set(ref this._isLoading, value); }
        }
        #endregion

    }
}
