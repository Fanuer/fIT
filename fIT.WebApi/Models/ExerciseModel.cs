using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fIT.WebApi.Models
{
  /// <summary>
  /// Defines one Exercise
  /// </summary>
    public class ExerciseModel: EntryModel<int>
    {
      [Display(ResourceType = typeof(Resources), Name = "Label_Description")]
      public string Description { get; set; }

      [Display(ResourceType = typeof(Resources), Name = "Label_Schedules")]
      public IEnumerable<EntryModel<int>> Schedules { get; set; }
    }
}
