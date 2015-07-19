using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Entities.Enums;

namespace fIT.WebApi.Models
{
    /// <summary>
    /// Data to register a User
    /// </summary>
    public class CreateUserModel
    {
        /// <summary>
        /// User name
        /// </summary>
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources),  Name = "Label_Username")]
        public string Username { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources), Name = "Label_Email")]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [StringLength(100, ErrorMessageResourceName = "Error_StringLength", ErrorMessageResourceType = typeof(Resources), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "Label_Password")]
        public string Password { get; set; }

        /// <summary>
        /// Confirm Password. Must be equal to the given Password
        /// </summary>
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "Label_ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceName = "Error_PasswordsNotEqual", ErrorMessageResourceType = typeof(Resources))]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Geschlecht
        /// </summary>        
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources), Name = "Label_Gender")]
        public GenderType Gender { get; set; }

        /// <summary>
        /// Berufsfeld
        /// </summary>
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources), Name = "Label_JobType")]
        public JobTypes Job { get; set; }

        /// <summary>
        /// Fittness
        /// </summary>
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources), Name = "Label_Fitness")]
        public FitnessType Fitness { get; set; }

        /// <summary>
        /// Alter
        /// </summary>
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources), Name = "Label_DateOfBirth")]
        public DateTime DateOfBirth { get; set; }
    }
}
