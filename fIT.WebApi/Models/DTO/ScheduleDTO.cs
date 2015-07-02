using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Models.DTO
{
    public class ScheduleDTO
    {
        #region ctor
        public ScheduleDTO(Schedule schedule)
        {
            if (schedule== null)
            {
                throw new ArgumentNullException("schedule");
            }
            this.ID = schedule.ID;
            this.Name = schedule.Name;
            this.UserID = schedule.UserID;
            this.Exercises = schedule.Exercises.Select(x=>x.ID).ToList();
        }

        public ScheduleDTO(int id = -1, string name = "", int userID = -1, ICollection<int> exercises = null)
        {
            this.ID = id;
            this.Name = name;
            this.UserID = userID;
            this.Exercises = exercises;
        }

        public ScheduleDTO()
            :this(-1)
        {

        }
        #endregion

        #region Properties
        /// <summary>
        /// DB ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// DisplayName des Trainingsplans
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Fremdschlüssel zum Nutzer
        /// </summary>        
        public int UserID { get; set; }

        /// <summary>
        /// Uebungen
        /// </summary>
        public virtual ICollection<int> Exercises { get; set; }

        #endregion
    }
}
