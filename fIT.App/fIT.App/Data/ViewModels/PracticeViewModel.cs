using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using fIT.App.Data.ViewModels.Abstract;

namespace fIT.App.Data.ViewModels
{
    public class PracticeViewModel:ListViewModel<PracticeListEntryItemViewModel>
    {
        #region CONST
        #endregion

        #region FIELDS
        #endregion

        #region CTOR
        public PracticeViewModel() : base("")
        {
        }
        #endregion

        #region METHODS
        protected override async Task OnAddClickedAsync()
        {
            var vm = new EditPracticeViewModel("Neuer Eintrag");
            await this.ViewModelNavigation.PushAsPopUpAsync(vm);
        }

        protected override async Task OnEditClickedAsync(int id)
        {
            var entry = this.List.First(x => x.Id == id);
            var vm = AutoMapper.Map<EditPracticeViewModel>(entry);
            vm.Title = "Eintrag bearbeiten";
            await this.ViewModelNavigation.PushAsPopUpAsync(vm);
        }

        protected override async Task OnRemoveClickedAsync(int id)
        {
            var answer = await this.UserDialogs.ConfirmAsync("Do you really want to delete this entry?");

            if (answer)
            {
                this.IsLoading = true;
                var um = await this.Repository.GetUserManagementAsync();
                await um.DeletePracticeAsync(id);
                var delete = this.List.First(x => x.Id == id);
                this.List.Remove(delete);
                this.IsLoading = false;
                this.ShowMessage($"Entry deleted", ToastEvent.Success);
            }
        }

        public override async Task InitAsync()
        {
            this.IsLoading = true;
            var um = await this.Repository.GetUserManagementAsync();
            var practices = await um.GetPracticesAsync(this.ScheduleId, this.Id.Value);
            this.List = new ObservableCollection<PracticeListEntryItemViewModel>(practices.Select(x =>
            {
                var result = this.AutoMapper.Map<PracticeListEntryItemViewModel>(x);
                result.ViewModelNavigation = this.ViewModelNavigation;
                return result;
            }));
            this.IsLoading = false;
        }

        #endregion

        #region PROPERTIES

        public int ScheduleId { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
