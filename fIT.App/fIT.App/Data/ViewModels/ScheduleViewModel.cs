using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper.Internal;
using fIT.App.Data.Datamodels;
using fIT.App.Interfaces;
using fIT.WebApi.Client.Data.Models.Schedule;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels
{
    public class ScheduleViewModel : AppViewModelBase
    {
        #region FIELDS

        private bool _IsRefreshing;
        private ObservableCollection<ScheduleModel> _ScheduleList;
        #endregion

        #region CTOR
        public ScheduleViewModel(IRepository rep) : base(rep, "Trainingspläne")
        {
            this._ScheduleList = new ObservableCollection<ScheduleModel>();
            this.OnRefresh = new Command(async () => await this.RefreshAsync(), () => !this.IsRefreshing);
        }

        #endregion

        #region METHODS

        protected override async Task InitAsync()
        {
            var um = await this.Repository.GetUserManagementAsync();
            var schedules = await um.GetAllSchedulesAsync();
            this.Schedules = new ObservableCollection<ScheduleModel>(schedules);
        }

        public async Task RefreshAsync()
        {
            await InitAsync();
            this.IsRefreshing = false;
        }

        #endregion

        #region PROPERTIES
        public ObservableCollection<ScheduleModel> Schedules
        {
            get { return this._ScheduleList; }
            set { this.Set(ref this._ScheduleList, value); }
        }

        public bool IsRefreshing
        {
            get { return this._IsRefreshing; }
            set { this.Set(ref this._IsRefreshing, value); }
        }

        public ICommand OnRefresh { get; private set; }

        #endregion
    }
}
