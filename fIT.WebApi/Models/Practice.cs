using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Key]
        [Column(Order = 1)]
        public int ID { get; set; }
        [Timestamp]
        public byte[] Timestamp { get; set; }
        public int Weight { get; set; }
        public int NumberOfRepetitions { get; set; }
        public int Repetitions { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ScheduleID { get; set; }
        public virtual Schedule Schedule { get; set; }

        [Key]
        [Column(Order = 3)]
        public int ExerciseID { get; set; }
        public virtual Exercise Exercise { get; set; }
        #endregion

    }
}
