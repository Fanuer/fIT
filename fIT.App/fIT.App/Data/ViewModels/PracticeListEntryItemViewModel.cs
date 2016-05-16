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
        public DateTime Timestamp { get; set; }
        public double Weight { get; set; }
        public int NumberOfRepetitions { get; set; }
        public int Repetitions { get; set; }

        public string Text => $"{this.NumberOfRepetitions} Sets à {this.Repetitions} x {this.Weight}kg";

        #endregion
    }
}
