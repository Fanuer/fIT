using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Repository.Interfaces.CRUD;

namespace fIT.WebApi.Entities
{
    /// <summary>
    /// Definiert eine Trainingseinheit
    /// </summary>
    public class Practice: IEntity<int>
    {
        public Practice(int id,
            int scheduleId,
            int exerciseId,
            string userId,
            DateTime timestamp = default(DateTime),
            double weight = 0,
            int repetitions = 0,
            int numberOfRepetitions = 0)
        {
            this.Id = id;
            this.UserId = userId;
            this.ScheduleId = scheduleId;
            this.ExerciseId = exerciseId;
            this.Timestamp = timestamp;
            this.Weight = weight;
            this.Repetitions = repetitions;
            this.NumberOfRepetitions = numberOfRepetitions;
        }

        public Practice():this(-1, -1, -1, "")
        {
            
        }

        #region Properties
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double Weight { get; set; }
        public int NumberOfRepetitions { get; set; }
        public int Repetitions { get; set; }
        public string UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ScheduleId { get; set; }

        [Key]
        [Column(Order = 3)]
        public int ExerciseId { get; set; }
        #endregion

    }
}
