using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels.Abstract;

namespace fIT.App.Data.ViewModels
{
    public class PracticeListEntryItemViewModel:ListEntryViewModel
    {
        #region CONST
        #endregion

        #region FIELDS
        private double _weight;
        private int _numberOfRepetitions;
        private int _repetitions;

        #endregion

        #region CTOR
        #endregion

        #region METHODS
        protected override Task OnEntryTappedAsync(int id)
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
        public string Text => $"{this.NumberOfRepetitions} Sets à {this.Repetitions} x {this.Weight}kg";

        #endregion
    }
}
