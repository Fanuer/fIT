using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace fIT.App.Data.Datamodels
{
    [Table("Schedule")]
    public class Schedule : LocalDataModelBase
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return $"[Schedule: LocalId={LocalId}, WasOffline={WasOffline}, Id={Id}, UserId={UserId}, Name={Name}]";
        }

        [Ignore]
        public int ExerciseCount { get; set; }
    }
}
