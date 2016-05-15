using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AutoMapper.Internal;
using fIT.App.Data.Datamodels;
using fIT.App.Data.ViewModels.Abstract;
using fIT.App.Helpers;
using fIT.App.Helpers.Navigation;
using fIT.App.Interfaces;
using fIT.WebApi.Client.Data.Models.Schedule;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public class ScheduleViewModel : ListViewModel<ScheduleListEntryViewModel>
    {
        #region FIELDS
        #endregion

        #region CTOR
        public ScheduleViewModel() : base("Trainingspläne")
        {
        }

        #endregion

        #region METHODS

        protected override async Task OnAddClickedAsync()
        {
            var vm = new EditScheduleViewModel();
            await this.ViewModelNavigation.PushAsPopUpAsync(vm);
        }

        protected override async Task OnEditClickedAsync(int id)
        {
            var vm = new EditScheduleViewModel(id, this.List.First(x => x.Id == id).Name);
            await this.ViewModelNavigation.PushAsPopUpAsync(vm);
        }

        protected override async Task OnRemoveClickedAsync(int id)
        {
            var answer = await this.UserDialogs.ConfirmAsync("Do you really want to delete this entry?");

            if (answer)
            {
                this.IsLoading = true;
                var um = await this.Repository.GetUserManagementAsync();
                await um.DeleteScheduleAsync(id);
                var delete = this.List.First(x => x.Id == id);
                this.List.Remove(delete);
                this.IsLoading = false;
                this.ShowMessage($"Schedule '{delete.Name}' deleted", ToastEvent.Success);
            }
        }
        protected override async Task InitAsync()
        {
            this.IsLoading = true;
            var um = await this.Repository.GetUserManagementAsync();
            var schedules = await um.GetAllSchedulesAsync();
            this.List = new ObservableCollection<ScheduleListEntryViewModel>(schedules.Select(x =>
            {
                var result = this.AutoMapper.Map<ScheduleListEntryViewModel>(x);
                result.ViewModelNavigation = this.ViewModelNavigation;
                return result;
            }));
            this.IsLoading = false;
        }

        #endregion

        #region PROPERTIES

        #endregion
    }
}
