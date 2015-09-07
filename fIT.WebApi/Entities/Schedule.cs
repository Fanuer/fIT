using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Repository.Interfaces.CRUD;

namespace fIT.WebApi.Entities
{
    /// <summary>
    /// Definiert einen Trainingsplan
    /// </summary>
    public class Schedule: IEntity<int>
    {
        #region ctor
        public Schedule(int id, string name = "", string userId = "", ICollection<Exercise> exercises = null)
        {
            this.Id = id;
            this.Name = name;
            this.UserID = userId;
            this.Exercises = exercises;
        }

        public Schedule()
            : this(-1)
        {

        }
        #endregion

        #region Properties
        /// <summary>
        /// DB ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// DisplayName des Trainingsplans
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Fremdschlüssel zum Nutzer (per Namens-Konvention) 
        /// </summary>        
        public string UserID { get; set; }

        /// <summary>
        /// Uebungen (per Namens-Konvention) 
        /// </summary>
        public virtual ICollection<Exercise> Exercises { get; set; }

        #endregion
    }
}
