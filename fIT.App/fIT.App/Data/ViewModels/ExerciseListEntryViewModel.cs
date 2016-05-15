using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using fIT.App.Data.ViewModels.Abstract;

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
        #endregion

        #region METHODS
        protected override async Task OnEntryTappedAsync(int id)
        {
            var vm = AutoMapper.Map<PracticeViewModel>(this);
            await this.ViewModelNavigation.PushAsync(vm);
        }
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Exercise Description
        /// </summary>
        public string Description
        {
            get{ return _description; }
            set { Set(ref _description, value); }
        }

        /// <summary>
        /// A short description to show in the List View
        /// </summary>
        public string ShortDescription => String.IsNullOrWhiteSpace(this.Description) && this.Description.Length > 255 ? $"{this.Description.Substring(0, 255)}..." : this.Description;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        #endregion
    }
}
