using fIT.WebApi;
using fIT.WebApi.Entities;
using fIT.WebApi.Entities.Enums;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Models
{
    public class RegisterUserModel
    {
        #region Methods
        internal UserInformation ToUserInfo()
        {
            return new UserInformation(age: this.Age, gender: this.Gender, fitness: this.Fitness, job: this.Job, username: this.UserName, email: this.Email);
        }
        #endregion

        #region Properties
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources), Name = "Label_Username")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources), Name = "Label_Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [StringLength(100, ErrorMessageResourceName = "Error_StringLength", ErrorMessageResourceType = typeof(Resources), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "Label_Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "Label_ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceName = "Error_PasswordsNotEqual", ErrorMessageResourceType = typeof(Resources))]
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
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        public int Age { get; set; }

        #endregion
    }
}
