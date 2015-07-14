using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fIT.WebApi.Models
{
  public class CreateRoleModel
  {
    [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
    [StringLength(256, ErrorMessageResourceName = "Error_StringLength", ErrorMessageResourceType = typeof(Resources), MinimumLength = 2)]
    [Display(ResourceType = typeof(Resources), Name = "Label_RoleName")]
    public string Name { get; set; }
  }
}