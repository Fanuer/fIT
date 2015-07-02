using fIT.WebApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace fIT.WebApi.Models.DTO
{
    public class ApplicationUserDTO
    {
        internal ApplicationUser ToDataModel()
        {
            return new ApplicationUser(this.ID, this.Name, this.Password, this.Email, this.Age, this.Gender.Value, this.Fitness.Value, this.Job.Value);
        }

        #region Ctor
        public ApplicationUserDTO(int id = -1,
                    string name = "",
                    string password = "",

                    int age = 0,
                    GenderType gender = GenderType.Male,
                    FitnessType fitness = FitnessType.NoSport,
                    JobTypes job = JobTypes.Middle)
        {
            this.ID = id;
            this.Name = name;
            this.Password = password;
            this.Age = age;
            this.Gender = gender;
            this.Fitness = fitness;
            this.Job = job;
        }

        public ApplicationUserDTO()
            :this(-1)
        {

        }


        public ApplicationUserDTO(ApplicationUser user)
        {
            if (user == null) { throw new ArgumentNullException("user"); }

            this.ID = user.ID;
            this.Email = user.Email;
            this.Name = user.Id;
            this.Password = user.Password;
            this.Age = user.Age;
            this.Gender = user.Gender;
            this.Fitness = user.Fitness;
            this.Job = user.Job;
            this.Schedules = user.Schedules.Select(x => x.ID);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Nutzer ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Email { get; set; }

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
        public virtual IEnumerable<int> Schedules { get; set; }

        #endregion

    }
}