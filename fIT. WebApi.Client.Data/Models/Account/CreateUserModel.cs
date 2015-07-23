using System;
using fIT.WebApi.Client.Data.Models.Shared.Enums;

namespace fIT.WebApi.Client.Data.Models.Account
{
    /// <summary>
    /// Data to register a User
    /// </summary>
    public class CreateUserModel
    {
        /// <summary>
        /// User name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Confirm Password. Must be equal to the given Password
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Geschlecht
        /// </summary>        
        public GenderType Gender { get; set; }

        /// <summary>
        /// Berufsfeld
        /// </summary>
        public JobTypes Job { get; set; }

        /// <summary>
        /// Fittness
        /// </summary>
        public FitnessType Fitness { get; set; }

        /// <summary>
        /// Alter
        /// </summary>
        public DateTime DateOfBirth { get; set; }
    }
}
