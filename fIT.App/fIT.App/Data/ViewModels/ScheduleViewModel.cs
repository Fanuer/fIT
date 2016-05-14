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
using fIT.App.Interfaces;
using fIT.App.Utilities.Navigation;
using fIT.WebApi.Client.Data.Models.Schedule;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public class ScheduleViewModel : ListViewModel<ScheduleListEntryViewModel>
    {
        #region FIELDS
        #endregion

        #region CTOR
        public ScheduleViewModel(IUserDialogs userDialogs) : base(userDialogs, "Trainingspläne")
        {
            this.OnAddClickedCommand = new Command(async () => await OnAddClickedAsync());
            this.OnEditClickedCommand = new Command<int>(async (id) => await OnEditClickedAsync(id));
            this.OnRemoveClickedCommand = new Command<int>(async (id) => await OnRemoveClickedAsync(id));
        }

        #endregion

        #region METHODS

        private async Task OnAddClickedAsync()
        {
            var vm = IoCLocator.Current.GetInstance<EditScheduleViewModel>();
            vm.ScheduleId = -1;
            vm.Name = "";
            await this.ViewModelNavigation.PushAsPopUpAsync(vm);
        }

        private async Task OnEditClickedAsync(int id)
        {
            var vm = IoCLocator.Current.GetInstance<EditScheduleViewModel>();
            vm.ScheduleId = id;
            vm.Name = this.List.First(x=>x.Id==id).Name;
            await this.ViewModelNavigation.PushAsPopUpAsync(vm);
        }

        private async Task OnRemoveClickedAsync(int id)
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
            }
        }

        protected override async Task InitAsync()
        {
            this.IsLoading = true;
            var um = await this.Repository.GetUserManagementAsync();
            var schedules = await um.GetAllSchedulesAsync();
            this.List = new ObservableCollection<ScheduleListEntryViewModel>(schedules.Select(x => this.AutoMapper.Map<ScheduleListEntryViewModel>(x)));
            this.IsLoading = false;
        }

        #endregion

        #region PROPERTIES

        #endregion
    }
}
