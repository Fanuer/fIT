using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Models
{
    /// <summary>
    /// Definiert eine Trainingseinheit
    /// </summary>
    public class Practice
    {
        #region Properties
        public int ID { get; set; }
        public DateTime Timestamp { get; set; }
        public int Weight { get; set; }
        public int NumberOfRepetitions { get; set; }
        public int Repetitions { get; set; }

        public int ScheduleID { get; set; }
        public Schedule Schedule { get; set; }

        public int ExerciseID { get; set; }
        public Exercise Exercise { get; set; }
        #endregion

    }
}
