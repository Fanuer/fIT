using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels.Abstract;
using fIT.WebApi.Client.Data.Models.Exercise;
using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public class EditExerciseViewModel:EditViewModel
    {
        #region CONST
        #endregion

        #region FIELDS

        private readonly IEnumerable<ExerciseModel> _newExercises;
        #endregion

        #region CTOR
        public EditExerciseViewModel(IEnumerable<ExerciseModel> newExercises) : base("Übung hinzufügen")
        {
            this._newExercises = newExercises;
        }

        #endregion

        #region METHODS
        protected override async Task HandleOkClickedAsync(ViewModelBase viewModel)
        {
            var exerciseModel = viewModel as ExerciseViewModel;
            if (exerciseModel != null)
            {
                var selectedExercise = this._newExercises.First(x => x.Name.Equals(SelectedExercise));
                var um = await this.Repository.GetUserManagementAsync();
                await um.AddExerciseToScheduleAsync(exerciseModel.Id.Value, selectedExercise.Id);
                var entry = AutoMapper.Map<ExerciseListEntryViewModel>(selectedExercise);
                exerciseModel.List.Add(entry);
            }
            else
            {
                throw new InvalidCastException($"The given ViewModel was expected to be of type '{nameof(ScheduleViewModel)}' but was '{viewModel.GetType().Name}'");
            }
        }
        #endregion

        #region PROPERTIES

        public IEnumerable<string> Exercises => this._newExercises.Select(x => x.Name);
        public string SelectedExercise { get; set; }
        #endregion

        
    }
}
