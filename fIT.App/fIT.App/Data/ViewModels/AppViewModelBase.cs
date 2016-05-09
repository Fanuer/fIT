using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.Datamodels;
using fIT.App.Interfaces;
using fIT.App.Utilities.Navigation.Interfaces;
using GalaSoft.MvvmLight;

namespace fIT.App.Data.ViewModels
{
    public abstract class AppViewModelBase : ViewModelBase, INavigatingViewModel
    {
        #region FIELDS

        private string _message;
        private string _title;
        #endregion

        #region CTOR

        protected AppViewModelBase(IRepository rep, string title)
        {
            if (rep == null)
            {
                throw new ArgumentNullException(nameof(rep));
            }
            this.Title = title;
            this.Repository = rep;
            this.Colors = new ColorViewModel();
            Task.Run(InitAsync);
        }
        #endregion

        #region METHODS

        protected virtual Task InitAsync()
        {
            return null;
        }

        #endregion

        #region PROPERTIES

        protected IRepository Repository { get; private set; }
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
        #endregion

    }
}
