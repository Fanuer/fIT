using System;
using fIT.WebApi.Entities.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fIT.WebApi.Entities
{
    /// <summary>
    /// Entität eines Nutzers
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
     
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
            this.Job = job;
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

        #region Method
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
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
        public virtual ICollection<Schedule> Schedules { get; set; }

        /// <summary>
        /// User Level
        /// </summary>
        [Required]
        public byte Level { get; set; }
        #endregion
    }
}
