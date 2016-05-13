using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AutoMapper;
using fIT.App.Interfaces;
using fIT.App.Utilities.Navigation;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public class ScheduleListEntryViewModel:AppViewModelBase
    {
        #region CONST
        
        #endregion

        #region FIELDS

        private string _name;
        private int _id;
        private int _count;
        #endregion

        #region CTOR

        public ScheduleListEntryViewModel(IUserDialogs userDialogs = null) : base(userDialogs, "")
        {
           this.OnEntryTappedCommand = new Command(async() => await OnEntryTapped()); 
        }
        #endregion

        #region METHODS
        private async Task OnEntryTapped()
        {
            await this.ViewModelNavigation.ExchangeAync(IoCLocator.Current.GetInstance<ScheduleViewModel>());
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// ExerciseID
        /// </summary>
        public int Id { get; set; }

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
        public int ExerciseCount {
            get
            {
                return _count;
            }
            set
            {
                Set(ref _count, value);
            }
        }

        public ICommand OnEntryTappedCommand { get; private set; }
        #endregion

    }
}
