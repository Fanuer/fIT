using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels.Abstract
{
    public abstract class ListEntryViewModel : AppViewModelBase
    {
        #region CONST
        #endregion

        #region FIELDS
        private int _id;

        #endregion

        #region CTOR
        protected ListEntryViewModel():base("")
        {
            this.OnEntryTabbedCommand = new Command<int>(async (id) => await OnEntryTappedAsync(id));
        }
        #endregion

        #region METHODS

        /*public T Clone<T>() where T : ListEntryViewModel
        {
            var type = this.GetType();
            var result = (T)Activator.CreateInstance(type);
            var properties = type.GetRuntimeProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CanWrite && propertyInfo.CanRead)
                {
                    propertyInfo.SetValue(result, propertyInfo.GetValue(this));
                }
            }
            return result;
        }*/

        protected abstract Task OnEntryTappedAsync(int id);

        #endregion

        #region PROPERTIES
        /// <summary>
        /// Entry Id
        /// </summary>
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                Set(ref _id, value);
            }
        }

        public ICommand OnEntryTabbedCommand { get; protected set; }

        public bool NavigationActive => this.ViewModelNavigation != null;

        #endregion
    }
}
