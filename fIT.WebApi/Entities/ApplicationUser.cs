using fIT.WebApi.Entities.Enums;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace fIT.WebApi.Entities
{
    /// <summary>
    /// Entität eines Nutzers
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        #region Method
        #endregion

        #region Ctor
        public ApplicationUser(
            string username = "",
            string email = "",
            int age = 0,
            GenderType gender = GenderType.Male,
            FitnessType fitness = FitnessType.NoSport,
            JobTypes job = JobTypes.Middle)
            : base(username)
        {
            Age = age;
            Gender = gender;
            Fitness = fitness;
            Email = email;
        }

        public ApplicationUser()
            : this("")
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Geschlecht
        /// </summary>
        [Required]
        public GenderType Gender { get; set; }

        /// <summary>
        /// Berufsfeld
        /// </summary>
        [Required]
        public JobTypes Job { get; set; }

        /// <summary>
        /// Fittness
        /// </summary>
        [Required]
        public FitnessType Fitness { get; set; }

        /// <summary>
        /// Alter
        /// </summary>
        [Required]
        public int Age { get; set; }

        /// <summary>
        /// Trainingsplaene
        /// </summary>
        //public virtual ICollection<Schedule> Schedules { get; set; }

        /// <summary>
        /// User Level
        /// </summary>
        [Required]
        public byte Level { get; set; }
        #endregion
    }
}
