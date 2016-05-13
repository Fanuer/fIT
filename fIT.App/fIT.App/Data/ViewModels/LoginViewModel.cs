using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AutoMapper;
using fIT.App.Helpers;
using fIT.App.Interfaces;
using fIT.App.Pages;
using fIT.App.Utilities.Navigation;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public class LoginViewModel : AppViewModelBase
    {
        #region FIELDS

        private string _username;
        private string _password;
        #endregion

        #region CTOR
        public LoginViewModel(IUserDialogs userdialogs) : base(userdialogs, "Login")
        {
            Username = "";
            Password = "";
            Message = "";

            this.OnLoginClickedCommand = new Command(async () => { await OnLoginClicked(); });
            this.OnSignUpClickedCommand = new Command(async () => await this.ViewModelNavigation.PushAsync(IoCLocator.Current.GetInstance<ScheduleViewModel>()));
        }

        #endregion

        #region METHODS

        private async Task OnLoginClicked()
        {
            this.Message = "";
            this.IsLoading = true;
            var result = await Repository.LoginAsync(this.Username, this.Password);
            this.Password = "";
            this.IsLoading = false;
            if (result)
            {
                await this.ViewModelNavigation.ExchangeAync(IoCLocator.Current.GetInstance<ScheduleViewModel>());
            }
            else
            {
                this.Message = "Login failed";
            }
        }

        #endregion

        #region PROPERTIES
        /// <summary>
        /// Login-Process-Command.
        /// Is fired, if the user has entered its user credentials and hits the login-button
        /// </summary>
        public ICommand OnLoginClickedCommand { get; private set; }

        /// <summary>
        /// Command is fired, if the user clicks on the 'Sign Up'-Button
        /// </summary>
        public ICommand OnSignUpClickedCommand { get; private set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                Set(ref _username, value);
            }
        }

        /// <summary>
        /// User Password to login
        /// </summary>
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
