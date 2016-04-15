using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.Datamodels;
using fIT.App.Interfaces;

namespace fIT.App.Data.ViewModels
{
  public class ScheduleViewModel:AppViewModelBase
  {
    #region FIELDS
    #endregion

    #region CTOR
    public ScheduleViewModel(IRepository rep) : base(rep)
    {
    }

    #endregion

    #region METHODS

    protected override async Task InitAsync()
    {
      Schedules = new ObservableCollection<Schedule>()
      {
        new Schedule() {Id=1, LocalId = 1, Name = "Test 1"},
        new Schedule() {Id=2, LocalId = 2, Name = "Test 2"},
      };
    }

    #endregion

    #region PROPERTIES
    public ObservableCollection<Schedule> Schedules { get; private set; }

    #endregion


  }
}
