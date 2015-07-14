using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fIT.WebApi.Models
{
  public class ScheduleModel:EntryModel<int>
  {
    public string UserId { get; set; }
    public IEnumerable<EntryModel<int>> Exercises { get; set; }
    
  }
}