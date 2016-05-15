using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace fIT.App.Data.ViewModels
{
    public class ExerciseViewModel:ListViewModel<ExerciseListEntryViewModel>
    {
        #region CONST
        #endregion

        #region FIELDS
        #endregion

        #region CTOR
        public ExerciseViewModel(string title) : base(title)
        {
        }
        #endregion

        #region METHODS
        protected override Task OnAddClickedAsync()
        {
            return null;
        }

        protected override Task OnEditClickedAsync(int id)
        {
            return null;
        }

        protected override Task OnRemoveClickedAsync(int id)
        {
            return null;
        }

        protected override async Task InitAsync()
        {
            this.IsLoading = true;
            var um = await this.Repository.GetUserManagementAsync();
            var models = await um.GetScheduleExercisesAsync(this.Id.Value);
            this.List = new ObservableCollection<ExerciseListEntryViewModel>(models.Select(x=>this.AutoMapper.Map<ExerciseListEntryViewModel>(x)));
            this.IsLoading = false;
        }

        #endregion

        #region PROPERTIES
        #endregion
    }
}
