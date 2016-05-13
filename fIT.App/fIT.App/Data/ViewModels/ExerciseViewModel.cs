using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace fIT.App.Data.ViewModels
{
    public class ExerciseViewModel:ListViewModel<ExerciseListViewModel>
    {
        #region CONST
        #endregion

        #region FIELDS
        #endregion

        #region CTOR
        public ExerciseViewModel(IUserDialogs userDialogs, string title) : base(userDialogs, title)
        {
        }
        #endregion

        #region METHODS
        #endregion

        #region PROPERTIES
        #endregion


    }
}
