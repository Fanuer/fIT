using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using GalaSoft.MvvmLight.Command;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public class EditScheduleViewModel:AppViewModelBase
    {
        #region CONST
        #endregion

        #region FIELDS
        private string _name;
        #endregion

        #region CTOR
        public EditScheduleViewModel(IUserDialogs userDialogs = null) : base(userDialogs, "Edit Schedule")
        {
            this.OnCancelClickCommand = new Command(async () => await this.ViewModelNavigation.PopModalAsync());
            this.OnOkClickCommand = new Command(async () => await this.EditScheduleAsync());
        }
        #endregion

        #region METHODS

        private async Task EditScheduleAsync()
        {
            var parent = await this.ViewModelNavigation.PopModalAsync();
            var scheduleModel = parent as ScheduleViewModel;
            if (scheduleModel != null)
            {
                var entry = scheduleModel.List.First(x => x.Id == this.ScheduleId);
                entry.Name = this.Name;
            }
        }


        #endregion

        #region PROPERTIES

        public int ScheduleId { get; set; }
        public ICommand OnOkClickCommand { get; private set; }
        public ICommand OnCancelClickCommand { get; private set; }
        
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        #endregion


    }
}
