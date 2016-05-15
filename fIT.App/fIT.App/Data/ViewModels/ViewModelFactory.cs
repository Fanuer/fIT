using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels.Abstract;

namespace fIT.App.Data.ViewModels
{
    public sealed class ViewModelFactory
    {
        #region CONST
        #endregion

        #region FIELDS
        private static ViewModelFactory _current = null;
        #endregion

        #region CTOR
        protected ViewModelFactory()
        {
        }
        #endregion

        #region METHODS

        public async Task CreateAsync<T>() where T : AppViewModelBase
        {
            return;
        }
        #endregion

        #region PROPERTIES
        public static ViewModelFactory Current => _current ?? (_current = new ViewModelFactory());
        #endregion
    }
}
