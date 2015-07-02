using fIT.WebApi.Models.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fIT.WebApi.Models
{
    /// <summary>
    /// Entität eines Nutzers
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        #region Method
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Beachten Sie, dass der "authenticationType" mit dem in "CookieAuthenticationOptions.AuthenticationType" definierten Typ übereinstimmen muss.
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Benutzerdefinierte Benutzeransprüche hier hinzufügen
            return userIdentity;
        }
        #endregion

        #region Ctor
        public ApplicationUser(
            int id = -1,
            string username = "",
            string password = "",
            string email = "",
            int age = 0,
            GenderType gender = GenderType.Male,
            FitnessType fitness = FitnessType.NoSport,
            JobTypes job = JobTypes.Middle) 
            : base(username)
        {
            ID = id;
            Email = Email;
            Password = password;
            Age = age;
            Gender = gender;
            Fitness = fitness;
        }

        public ApplicationUser()
            : this(-1)
        {

        }

        #endregion

        #region Properties
        /// <summary>
        /// Nutzer ID
        /// </summary>
        public int ID { get; set; }

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
