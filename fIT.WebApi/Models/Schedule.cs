using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Models
{
    /// <summary>
    /// Definiert einen Trainingsplan
    /// </summary>
    public class Schedule
    {
        #region ctor
        public Schedule(int id = -1, string name = "", int userID = -1, User owner = null, ICollection<Exercise> exercises = null)
        {
            this.ID = id;
            this.Name = name;
            this.UserID = userID;
            this.User = owner;
            this.Exercises = exercises;
        }

        public Schedule()
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
        /// Name des Trainingsplans
        /// </summary>
        [Required]        
        public string Name { get; set; }

        /// <summary>
        /// Fremdschlüssel zum Nutzer
        /// </summary>        
        public int UserID { get; set; }

        /// <summary>
        /// Nutzer
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Uebungen
        /// </summary>
        public virtual ICollection<Exercise> Exercises { get; set; }

        #endregion
    }
}
