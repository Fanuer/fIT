using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AutoMapper;
using fIT.App.Helpers;
using fIT.App.Interfaces;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public class ScheduleListEntryViewModel : ListEntryViewModel
    {
        #region CONST

        #endregion

        #region FIELDS

        private string _name;
        private int _count;
        #endregion

        #region CTOR

        public ScheduleListEntryViewModel()
        {
        }
        #endregion

        #region METHODS
        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// Name of the owning Schedule
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Set(ref _name, value);
            }
        }
        /// <summary>
        /// Number of Exercises
        /// </summary>
        public int ExerciseCount
        {
            get
            {
                return _count;
            }
            set
            {
                Set(ref _count, value);
            }
        }
        #endregion

        protected override async Task OnEntryTappedAsync(int id)
        {
            var vm = AutoMapper.Map<ExerciseViewModel>(this);
            await this.ViewModelNavigation.PushAsync(vm);
        }
    }
}
