using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public abstract class ListViewModel : AppViewModelBase
    {
        #region CONST
        #endregion

        #region FIELDS
        private bool _isRefreshing;
        protected ObservableCollection<ListEntryViewModel> _list;

        #endregion

        #region CTOR

        protected ListViewModel(IUserDialogs userDialogs, string title) : base(userDialogs, title)
        {
            this.OnRefreshCommand = new Command(async () => await this.RefreshAsync(), () => !this.IsRefreshing);
        }
        #endregion

        #region METHODS
        public virtual async Task RefreshAsync()
        {
            await InitAsync();
            this.IsRefreshing = false;
        }

        #endregion

        #region PROPERTIES
        public virtual ObservableCollection<ListEntryViewModel> List
        {
            get { return this._list; }
            set { this.Set(ref this._list, value); }
        }

        public bool IsRefreshing
        {
            get { return this._isRefreshing; }
            set { this.Set(ref this._isRefreshing, value); }
        }
        public ICommand OnRefreshCommand { get; protected set; }
        public ICommand OnAddClickedCommand { get; protected set; }

        #endregion


    }

    public abstract class ListViewModel<T>: ListViewModel where T: ListEntryViewModel
    {
        #region CONST
        #endregion

        #region FIELDS
        #endregion

        #region CTOR

        protected ListViewModel(IUserDialogs userDialogs, string title) : base(userDialogs, title)
        {
            List = new ObservableCollection<T>();
        }
        #endregion

        #region METHODS

        #endregion

        #region PROPERTIES
        public new ObservableCollection<T> List
        {
            get
            {
                return base.List != null ? new ObservableCollection<T>(base.List.Cast<T>()) : null;
            }
            set
            {
                if (value!= null)
                {
                    this.Set(ref _list, new ObservableCollection<ListEntryViewModel>(value));
                }
            }
        }

        #endregion


    }
}
