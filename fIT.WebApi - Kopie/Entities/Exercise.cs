using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Entities
{
    /// <summary>
    /// Uebung fuer Trainingspläne
    /// </summary>
    public class Exercise
    {
        #region ctor
        public Exercise(int id = -1, string name ="", string description ="", ICollection<Schedule> schedules = null)
        {
            this.ID = id;
            this.Name = name;
            this.Description = description;
            this.Schedules = Schedules;
        }

        public Exercise()
            :this(-1)
        {

        }
        #endregion

        #region Properties
        /// <summary>
        /// ID der Uebung
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// DisplayName
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Beschreibung
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Trainingsplaene, die diese Uebung enthalten
        /// </summary>
        public virtual ICollection<Schedule> Schedules { get; set; }
        #endregion

    }
}
