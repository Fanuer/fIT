using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using fIT.App.Helpers;
using fIT.App.Utilities.Navigation;
using fIT.WebApi.Client.Data.Models.Schedule;
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
            this.OnCancelClickCommand = new Command(async () => await this.ViewModelNavigation.PopAsPopUpAsync());
            this.OnOkClickCommand = new Command(async () => await this.HandleScheduleChange());
        }
        #endregion

        #region METHODS

        private async Task HandleScheduleChange()
        {
            var parent = await this.ViewModelNavigation.PopAsPopUpAsync();
            var scheduleModel = parent as ScheduleViewModel;
            if (scheduleModel != null)
            {
                scheduleModel.IsLoading = true;
                if (this.ScheduleId == -1)
                {
                    await this.AddScheduleAsync(scheduleModel);
                }
                else
                {
                    await this.EditScheduleAsync(scheduleModel);
                }
                scheduleModel.IsLoading = false;
            }
        }

        private async Task EditScheduleAsync(ScheduleViewModel vm)
        {
            var um = await Repository.GetUserManagementAsync();
            var schedule = await um.GetScheduleByIdAsync(this.ScheduleId);
            schedule.Name = this.Name;
            await um.UpdateScheduleAsync(this.ScheduleId, schedule);
            vm.List.First(x => x.Id == this.ScheduleId).Name = this.Name;
            
        }

        private async Task AddScheduleAsync(ScheduleViewModel vm)
        {
            try
            {
                var um = await Repository.GetUserManagementAsync();
                var newSchedule = await um.CreateScheduleAsync(new ScheduleModel()
                {
                    Name = this.Name,
                    UserId = Settings.UserId.ToString()
                });
                var newEntry = AutoMapper.Map<ScheduleListEntryViewModel>(newSchedule);
                vm.List.Add(newEntry);
            }
            catch (Exception e)
            {
                throw e;
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
