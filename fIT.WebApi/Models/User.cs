using fIT.WebApi.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fIT.WebApi.Models
{
    /// <summary>
    /// Entität eines Nutzers
    /// </summary>
    public class User
    {
        #region Ctor
        public User(int id = -1,
                    string name = "",
                    string password = "",
                    int age = 0,
                    GenderType gender = GenderType.Male,
                    FitnessType fitness = FitnessType.NoSport)
        {
            this.ID = id;
            this.Name = name;
            this.Password = password;
            this.Age = age;
            this.Gender = gender;
            this.Fitness = fitness;
        }

        public User()
            :this(-1)
        {

        }

        #endregion

        #region Properties
        /// <summary>
        /// Nutzer ID
        /// </summary>
        public int ID { get; set; }
        
        /// <summary>
        /// Name des Nutzers
        /// </summary>
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Geschlecht
        /// </summary>
        public GenderType? Gender { get; set; }
        
        /// <summary>
        /// Passwort des Nutzers
        /// </summary>
        [Required]
        public string Password { get; set; }
        
        /// <summary>
        /// Berufsfeld
        /// </summary>
        public JobTypes? Job { get; set; }
        
        /// <summary>
        /// Fittness
        /// </summary>
        public FitnessType? Fitness { get; set; }
        
        /// <summary>
        /// Alter
        /// </summary>
        [Required]
        public int Age { get; set; }

        /// <summary>
        /// Trainingsplaene
        /// </summary>
        public virtual ICollection<Schedule> Schedules { get; set; }

        #endregion
    }
}
