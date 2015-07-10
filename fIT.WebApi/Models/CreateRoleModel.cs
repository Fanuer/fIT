using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace fIT.WebApi.Models
{
  public class CreateRoleModel
  {
    [Required]
    [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
    [Display(Name = "Role Name")]
    public string Name { get; set; }
  }
}