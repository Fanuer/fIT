using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using fIT.WebApi.Client.Data.Models.Shared;

namespace fIT.WebApi.Client.Data.Models.Exercise
{
  /// <summary>
  /// Defines one Exercise
  /// </summary>
    public class ExerciseModel: EntryModel<int>
    {
      public string Description { get; set; }
      public IEnumerable<EntryModel<int>> Schedules { get; set; }
    }
}
