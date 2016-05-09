using System.Collections.Generic;
using fIT.WebApi.Client.Data.Models.Shared;

namespace fIT.WebApi.Client.Data.Models.Schedule
{
    public class ScheduleModel : EntryModel<int>
    {
        #region ctor

        public ScheduleModel()
        {
            this.Exercises = new List<EntryModel<int>>();
        }
        #endregion

        #region Properties
        public string UserId { get; set; }
        public ICollection<EntryModel<int>> Exercises { get; set; } 
        #endregion
    }
}