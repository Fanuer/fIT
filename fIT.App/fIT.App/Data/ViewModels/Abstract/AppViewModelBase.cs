using System.Threading.Tasks;
using Acr.UserDialogs;
using AutoMapper;
using fIT.App.Helpers;
using fIT.App.Helpers.Navigation.Interfaces;
using fIT.App.Interfaces;
using GalaSoft.MvvmLight;

namespace fIT.App.Data.ViewModels.Abstract
{
    public abstract class AppViewModelBase : ViewModelBase, INavigatingViewModel
    {
        #region FIELDS

        private string _message;
        private string _title;
        private bool _isLoading;
        #endregion

        #region CTOR

        protected AppViewModelBase(string title)
        {
            this.Title = title;
            this.Colors = new ColorViewModel();
            this.Images = new ImageViewModel();
            this.UserDialogs = IoCLocator.Current.GetInstance<IUserDialogs>();
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
            Settings.RemoveUserSettings();
            await this.ViewModelNavigation.PopToRootAsync();
            //await this.ViewModelNavigation.ExchangeAync(IoCLocator.Current.GetInstance<LoginViewModel>());
            await this.ViewModelNavigation.ExchangeAync(new LoginViewModel());
        }

        /// <summary>
        /// Shows Message that provides feedback to a user
        /// </summary>
        /// <param name="message">Feedback-Message</param>
        /// <param name="type">Type of Message</param>
        /// <param name="description">Additional Information</param>
        public void ShowMessage(string message, ToastEvent type = ToastEvent.Error, string description = null)
        {
            this.UserDialogs.Toast(new ToastConfig(type, message, description));
        }
        #endregion

        #region PROPERTIES

        protected IRepository Repository => IoCLocator.Current.GetInstance<IRepository>();
        protected IUserDialogs UserDialogs { get; private set; }
        protected IMapper AutoMapper => IoCLocator.Current.GetInstance<IMapper>();

        /// <summary>
        /// Page Title
        /// </summary>
        public string Title
        {
            get { return _title; }
            internal set { this.Set(ref this._title, value); }
        }

        /// <summary>
        /// Navigation
        /// </summary>
        public IViewModelNavigation ViewModelNavigation { get; set; }

        public ColorViewModel Colors { get; private set; }

        public ImageViewModel Images{ get; private set; }

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
