using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using fIT.App.Data.ViewModels.Abstract;
using fIT.App.Helpers;
using fIT.WebApi.Client.Data.Models.Schedule;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public class EditScheduleViewModel: EditViewModel
    {
        #region CONST
        #endregion

        #region FIELDS
        private string _name;
        #endregion

        #region CTOR
        public EditScheduleViewModel(int scheduleId = -1, string name = "") : base("Trainingsplan ändern")
        {
            this.ScheduleId = scheduleId;
            this.Name = name;
        }
        #endregion

        #region METHODS

        protected override async Task HandleOkClickedAsync(ViewModelBase viewModel)
        {
            var scheduleModel = viewModel as ScheduleViewModel;
            if (viewModel != null)
            {
                scheduleModel.IsLoading = true;
                if (this.ScheduleId == -1)
                {
                    await this.AddScheduleAsync(scheduleModel);
                    this.ShowMessage($"Schedule '{this.Name}' created", ToastEvent.Success);
                }
                else
                {
                    await this.EditScheduleAsync(scheduleModel);
                    this.ShowMessage($"Schedule '{this.Name}' updated", ToastEvent.Success);
                }
            }
            else
            {
                throw new InvalidCastException($"The given ViewModel was expected to be of type '{nameof(ScheduleViewModel)}' but was '{viewModel.GetType().Name}'");
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
                newEntry.ViewModelNavigation = this.ViewModelNavigation;
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
        
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        #endregion

        
    }
}
