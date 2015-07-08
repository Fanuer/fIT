using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Entities.Enums;

namespace fIT.WebApi.Models
{
    public class RegisterUserModel
    {
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources),  Name = "Label_Username")]
        public string Username { get; set; }

        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [Display(ResourceType = typeof(Resources), Name = "Label_Email")]
        [EmailAddress]
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
        [Display(ResourceType = typeof(Resources), Name = "Label_Age")]
        public int Age { get; set; }
    }
}
