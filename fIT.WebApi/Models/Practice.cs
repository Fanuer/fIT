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
        public Practice(int id, 
            int scheduleID, 
            int exerciseID, 
            byte[] timestamp = null, 
            int weight = 0, 
            int repetitions = 0, 
            int numberOfRepetitions = 0)
        {
            this.ID = id;
            this.ScheduleID = scheduleID;
            this.ExerciseID = exerciseID;
            this.Timestamp = timestamp;
            this.Weight = weight;
            this.Repetitions = repetitions;
            this.NumberOfRepetitions = numberOfRepetitions;
        }

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

        [Key]
        [Column(Order = 3)]
        public int ExerciseID { get; set; }
        #endregion

    }
}
