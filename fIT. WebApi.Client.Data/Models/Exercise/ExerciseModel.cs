using System.Collections.Generic;
using fIT.WebApi.Client.Data.Models.Shared;

namespace fIT.WebApi.Client.Data.Models.Exercise
{
    /// <summary>
    /// Defines one Exercise
    /// </summary>
    public class ExerciseModel : EntryModel<int>
    {
        #region Ctor

        public ExerciseModel()
        {
            Schedules = new List<EntryModel<int>>();
        }
        #endregion

        #region Properties
        public string Description { get; set; }
        public IEnumerable<EntryModel<int>> Schedules { get; set; }
        #endregion
    }
}
