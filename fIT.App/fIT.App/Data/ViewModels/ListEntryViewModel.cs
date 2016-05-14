using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;

namespace fIT.App.Data.ViewModels
{
    public abstract class ListEntryViewModel : AppViewModelBase
    {
        #region CONST
        #endregion

        #region FIELDS
        #endregion

        #region CTOR
        protected ListEntryViewModel(IUserDialogs userDialogs, string title) : base(userDialogs, title)
        {
        }
        #endregion

        #region METHODS
        #endregion

        #region PROPERTIES
        public ICommand OnEntryTappedCommand { get; protected set; }
        #endregion


    }
}
