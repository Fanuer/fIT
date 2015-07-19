using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Models
{
    /// <summary>
    /// Model to change a password
    /// </summary>
    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "Label_CurrentPassword")]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [StringLength(100, ErrorMessageResourceName = "Error_StringLength", ErrorMessageResourceType = typeof(Resources), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "Label_NewPassword")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources), Name = "Label_ConfirmPassword")]
        [Compare("NewPassword", ErrorMessageResourceName = "Error_PasswordsNotEqual", ErrorMessageResourceType = typeof(Resources))]
        public string ConfirmPassword { get; set; }
    }
}
