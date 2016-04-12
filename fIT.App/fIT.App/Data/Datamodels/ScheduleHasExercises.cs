using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace fIT.App.Data.Datamodels
{
  [Table("ScheduleHasExercises")]
  class ScheduleHasExercises:LocalDataModelBase
  {
    public int ScheduleId { get; set; }
    public int ExerciseId { get; set; }
    public override string ToString()
    {
      return $"[ScheduleHasExercises: LocalId={LocalId}, WasOffline={WasOffline}, ScheduleId={ScheduleId}, ExerciseId={ExerciseId}]";
    }

  }
}
