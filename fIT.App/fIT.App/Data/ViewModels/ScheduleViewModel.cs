using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }

        #endregion

        #region METHODS

        private async Task OnAddClickedAsync()
        {
            var vm = IoCLocator.Current.GetInstance<EditScheduleViewModel>();
            vm.ScheduleId = -1;
            vm.Name = "";
            await this.ViewModelNavigation.PushModalAsync(vm);
        }

        protected override async Task InitAsync()
        {
            var um = await this.Repository.GetUserManagementAsync();
            var schedules = await um.GetAllSchedulesAsync();
            this.List = new ObservableCollection<ScheduleListEntryViewModel>(schedules.Select(x =>
            {
                var result = this.AutoMapper.Map<ScheduleListEntryViewModel>(x);
                result.Owner = this;
                return result;
            }));
        }

        #endregion

        #region PROPERTIES
        #endregion
    }
}
