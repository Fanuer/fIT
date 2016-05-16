using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using fIT.App.Data.ViewModels.Abstract;
using fIT.App.Helpers;
using fIT.WebApi.Client.Data.Models.Practice;
using GalaSoft.MvvmLight;

namespace fIT.App.Data.ViewModels
{
    public class EditPracticeViewModel:EditViewModel
    {
        #region CONST
        #endregion

        #region FIELDS
        private double _weight;
        private int _numberOfRepetitions;
        private int _repetitions;

        #endregion

        #region CTOR
        public EditPracticeViewModel(string title) : base(title)
        {
            this.Weight = this.MinWeight;
            this.NumberOfRepetitions = this.MinNumberOfRepetitions + ((this.MaxNumberOfRepetitions - this.MinNumberOfRepetitions) / 2);
            this.Repetitions = this.MinRepetitions + ((this.MaxRepetitions - this.MinRepetitions) / 2);
        }
        #endregion

        #region METHODS
        protected override async Task HandleOkClickedAsync(ViewModelBase viewModel)
        {
            var practiceModel = viewModel as PracticeViewModel;
            try
            {
                if (practiceModel != null)
                {
                    var um = await this.Repository.GetUserManagementAsync();
                    if (this.Id == -1)
                    {
                        var model = AutoMapper.Map<PracticeModel>(this);
                        model.Timestamp = DateTime.Now;
                        model.UserId = Settings.UserId.ToString();
                        var result = await um.CreatePracticeAsync(model);

                        var newEntry = AutoMapper.Map<PracticeListEntryItemViewModel>(result);
                        newEntry.ViewModelNavigation = this.ViewModelNavigation;
                        practiceModel.List.Insert(0, newEntry);

                        this.ShowMessage("New Entry created", ToastEvent.Success);
                    }
                    else
                    {
                        var model = await um.GetPracticeByIdAsync(this.Id);
                        model.NumberOfRepetitions = this.NumberOfRepetitions;
                        model.Weight = this.Weight;
                        model.Repetitions = this.Repetitions;
                        await um.UpdatePracticeAsync(this.Id, model);

                        var entry = practiceModel.List.First(x => x.Id == this.Id);
                        entry.NumberOfRepetitions = this.NumberOfRepetitions;
                        entry.Repetitions = this.Repetitions;
                        entry.Weight = this.Weight;

                        this.ShowMessage("Entry updated", ToastEvent.Success);
                    }
                }
            }
            catch (Exception e)
            {
                ShowMessage("Error occured");
            }
        }

        
        #endregion

        #region PROPERTIES

        public string Text => $"{this.NumberOfRepetitions} Sets à {this.Repetitions} x {this.Weight}kg";

        public double Weight
        {
            get { return _weight; }
            set
            {
                Set(ref _weight, value);
                this.RaisePropertyChanged("Text");
            }
        }
        public int NumberOfRepetitions
        {
            get { return _numberOfRepetitions; }
            set
            {
                Set(ref _numberOfRepetitions, value);
                this.RaisePropertyChanged("Text");
            }
        }
        public int Repetitions
        {
            get { return _repetitions; }
            set
            {
                Set(ref _repetitions, value);
                this.RaisePropertyChanged("Text");
            }
        }

        public DateTime Timestamp { get; set; }

        public double MinWeight => 1d;
        public double MaxWeight => 150d;
        public double StepSizeWeight => 0.5d;
        public int MinNumberOfRepetitions => 1;
        public int MaxNumberOfRepetitions => 10;
        public int MinRepetitions => 1;
        public int MaxRepetitions => 50;
        public int ScheduleId { get; set; }
        public int ExerciseId { get; set; }
        public int Id { get; set; }
        #endregion
    }
}
