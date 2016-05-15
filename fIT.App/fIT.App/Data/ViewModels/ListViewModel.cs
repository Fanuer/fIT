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
    public abstract class ListViewModel<T> : AppViewModelBase where T : ListEntryViewModel
    {
        #region CONST
        #endregion

        #region FIELDS
        private bool _isRefreshing;
        private ObservableCollection<T> _list;
        private int? _id;
        #endregion

        #region CTOR

        protected ListViewModel(string title):base(title)
        {
            this._isRefreshing = false;
            this._id = -1;
            this._list = new ObservableCollection<T>();
            this.OnRefreshCommand = new Command(async () => await this.RefreshAsync(), () => !this.IsRefreshing);
            this.OnAddClickedCommand = new Command(async () => await OnAddClickedAsync());
            this.OnEditClickedCommand = new Command<int>(async (id) => await OnEditClickedAsync(id));
            this.OnRemoveClickedCommand = new Command<int>(async (id) => await OnRemoveClickedAsync(id));
            //this.OnEntryTabbedCommand = new Command<int>(async (id) => await OnEntryTappedAsync(id));
        }
        #endregion

        #region METHODS
        protected virtual async Task RefreshAsync()
        {
            await InitAsync();
            this.IsRefreshing = false;
        }

        protected abstract Task OnAddClickedAsync();

        protected abstract Task OnEditClickedAsync(int id);

        protected abstract Task OnRemoveClickedAsync(int id);

        #endregion

        #region PROPERTIES
        public ObservableCollection<T> List
        {
            get
            {
                return _list;
            }
            set
            {
                if (value != null)
                {
                    this.Set(ref _list, value);
                }
            }
        }

        /// <summary>
        /// Checks wheather the page is currently refreshing
        /// </summary>
        public bool IsRefreshing
        {
            get { return this._isRefreshing; }
            set { this.Set(ref this._isRefreshing, value); }
        }
        /// <summary>
        /// Action that is performed when the user refreshed the page by swiping down
        /// </summary>
        public ICommand OnRefreshCommand { get; protected set; }

        public ICommand OnAddClickedCommand { get; protected set; }
        public ICommand OnRemoveClickedCommand { get; protected set; }
        public ICommand OnEditClickedCommand { get; protected set; }

        /// <summary>
        /// Optional Id of the current Entity (used to make webserver-calls)
        /// </summary>
        public int? Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }
        
        #endregion
    }
}
