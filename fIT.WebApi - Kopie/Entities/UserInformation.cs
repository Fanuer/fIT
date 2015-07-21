using fIT.WebApi.Entities.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fIT.WebApi.Entities
{
    /// <summary>
    /// Entität eines Nutzers
    /// </summary>
    public class UserInformation:IdentityUser
    {
        #region Method
        #endregion

        #region Ctor
        public UserInformation(            
            string username = "",
            string email = "",
            int age = 0,
            GenderType gender = GenderType.Male,
            FitnessType fitness = FitnessType.NoSport,
            JobTypes job = JobTypes.Middle, 
            int id = -1) 
            :base(username)
        {
            UserID = id;
            Age = age;
            Gender = gender;
            Fitness = fitness;
            Email = email;            
        }

        public UserInformation()
            : this("")
        {

        }

        #endregion

        #region Properties
        /// <summary>
        /// Nutzer ID
        /// </summary>
        [Index(IsUnique = true)]
        public int UserID { get; set; }

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
        public virtual ICollection<Schedule> Schedules { get; set; }
        
        #endregion
    }
}
