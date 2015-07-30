using System.Collections.Generic;

namespace fIT.WebApi.Client.Data.Models.Schedule
{
  public class ScheduleModel:EntryModel<int>
  {
    public string UserId { get; set; }
    public IEnumerable<EntryModel<int>> Exercises { get; set; }
    
  }
}