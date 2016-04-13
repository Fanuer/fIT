using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace fIT.App.Data.Datamodels
{
  [Table("Practice")]

  public class Practice:LocalDataModelBase
  {
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public double Weight { get; set; }
    public int NumberOfRepetitions { get; set; }
    public int Repetitions { get; set; }
    //Foreign Key
    public int ScheduleId { get; set; }
    //Foreign Key
    public int ExerciseId { get; set; }
    //Foreign Key
    public string UserId { get; set; }
    public override string ToString()
    {
      return $"[Exercise: LocalId={LocalId}, WasOffline={WasOffline}, Id={Id}, Timestamp={Timestamp}, Weight={Weight}, NumberOfRepetitions={NumberOfRepetitions}, Repetitions={Repetitions}, ScheduleId={ScheduleId}, ExerciseId={ExerciseId}, UserId={UserId}";
    }

  }
}
