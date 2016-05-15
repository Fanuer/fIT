using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace fIT.App.Data.ViewModels
{
    public class ExerciseListEntryViewModel:ListEntryViewModel
    {
        #region CONST
        #endregion

        #region FIELDS
        private string _description;
        private string _name;
        #endregion

        #region CTOR
        public ExerciseListEntryViewModel()
        {
        }
        #endregion

        #region METHODS
        #endregion

        #region PROPERTIES
        
        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }
        

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        #endregion

        protected override Task OnEntryTappedAsync(int id)
        {
            return null;
        }
    }
}
