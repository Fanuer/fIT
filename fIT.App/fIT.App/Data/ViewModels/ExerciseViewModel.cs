using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using AutoMapper.Internal;
using fIT.App.Data.ViewModels.Abstract;
using fIT.WebApi.Client.Data.Models.Exercise;

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
        protected override async Task OnAddClickedAsync()
        {
            var um = await this.Repository.GetUserManagementAsync();
            var exercises = await um.GetAllExercisesAsync();
            var newExercises = exercises.Where(exercise => !exercise.Schedules.Any(schedule => schedule.Id == this.Id.Value));
            var vm = new EditExerciseViewModel(newExercises);
            await vm.InitAsync();
            await this.ViewModelNavigation.PushAsPopUpAsync(vm);
        }

        protected override Task OnEditClickedAsync(int id)
        {
            var result = new Task(() => { });
            return result;
        }

        protected override async Task OnRemoveClickedAsync(int id)
        {
            var answer = await this.UserDialogs.ConfirmAsync("Do you really want to delete this entry?");

            if (answer)
            {
                this.IsLoading = true;
                var um = await this.Repository.GetUserManagementAsync();
                await um.RemoveExerciseFromScheduleAsync(this.Id.Value, id);
                var delete = this.List.First(x => x.Id == id);
                this.List.Remove(delete);
                this.IsLoading = false;
                this.ShowMessage($"Übung '{delete.Name}' entfernt", ToastEvent.Success);
            }
        }

        public override async Task InitAsync()
        {
            this.IsLoading = true;
            var um = await this.Repository.GetUserManagementAsync();
            var models = await um.GetScheduleExercisesAsync(this.Id.Value);
            this.List = new ObservableCollection<ExerciseListEntryViewModel>(models.Select(x=>this.AutoMapper.Map<ExerciseListEntryViewModel>(x)));
            this.List.Each(x =>
            {
                x.ScheduleId = this.Id.Value;
                x.ViewModelNavigation = this.ViewModelNavigation;
            });
            this.IsLoading = false;
        }

        #endregion

        #region PROPERTIES
        #endregion
    }
}
