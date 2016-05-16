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

        private List<ExerciseModel> _newExercises;
        #endregion

        #region CTOR
        public EditExerciseViewModel(IEnumerable<ExerciseModel> newExercises) : base("Übung hinzufügen")
        {
            if (newExercises == null)
            {
                throw new ArgumentNullException(nameof(newExercises), "No Exercises to use as Picker options were passed");
            }
            this.NewExercises = newExercises.ToList();
            this.SelectedExercise = NewExercises.FirstOrDefault();
        }

        #endregion

        #region METHODS
        protected override async Task HandleOkClickedAsync(ViewModelBase viewModel)
        {
            var exerciseModel = viewModel as ExerciseViewModel;
            if (exerciseModel != null && SelectedExercise != null)
            {
                var um = await this.Repository.GetUserManagementAsync();
                await um.AddExerciseToScheduleAsync(exerciseModel.Id.Value, SelectedExercise.Id);
                var entry = AutoMapper.Map<ExerciseListEntryViewModel>(SelectedExercise);
                exerciseModel.List.Add(entry);
            }
            else
            {
                throw new InvalidCastException($"The given ViewModel was expected to be of type '{nameof(ScheduleViewModel)}' but was '{viewModel.GetType().Name}'");
            }
        }
        #endregion

        #region PROPERTIES
        public List<ExerciseModel> NewExercises
        {
            get { return _newExercises; }
            set { Set(ref _newExercises, value); }
        }

        public ExerciseModel SelectedExercise { get; set; }
        #endregion

        
    }
}
