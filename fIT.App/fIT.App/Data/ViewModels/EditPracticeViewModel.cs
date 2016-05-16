using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels.Abstract;
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
            this._weight = this.MinWeight;
            this.NumberOfRepetitions = this.MinNumberOfRepetitions + Convert.ToInt32((this.MaxNumberOfRepetitions - this.MinNumberOfRepetitions) / 2);
            this.Repetitions = this.MinRepetitions + Convert.ToInt32((this.MaxRepetitions - this.MinRepetitions) / 2);
        }
        #endregion

        #region METHODS
        protected override Task HandleOkClickedAsync(ViewModelBase viewModel)
        {
            return Task.Run(() => { });
        }
        #endregion

        #region PROPERTIES

        public double Weight
        {
            get { return _weight; }
            set
            {
                if (value > MaxWeight)
                {
                    _weight = MaxWeight;
                }
                else if (value < MinWeight)
                {
                    _weight = MinWeight;
                }
                else
                {
                    _weight = value;
                }
            }
        }


        public int NumberOfRepetitions
        {
            get { return _numberOfRepetitions; }
            set
            {
                if (value > MinNumberOfRepetitions)
                {
                    _numberOfRepetitions = MinNumberOfRepetitions;
                }
                else if (value < MaxNumberOfRepetitions)
                {
                    _numberOfRepetitions = MaxNumberOfRepetitions;
                }
                else
                {
                    _numberOfRepetitions = value;
                }
            }
        }

        public int Repetitions
        {
            get { return _repetitions; }
            set
            {
                if (value > MinRepetitions)
                {
                    _repetitions = MinRepetitions;
                }
                else if (value < MaxRepetitions)
                {
                    _repetitions = MinRepetitions;
                }
                else
                {
                    _repetitions = value;
                }
                _repetitions = value;
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
        #endregion
    }
}
