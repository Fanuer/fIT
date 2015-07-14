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
  /// User data
  /// </summary>
  public class UserModel
  {
    /// <summary>
    /// Url to receive this object
    /// </summary>
    [Display(ResourceType = typeof(Resources), Name = "Label_Url")]
    public string Url { get; set; }
    /// <summary>
    /// User Id
    /// </summary>
    [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
    [Display(ResourceType = typeof(Resources), Name = "Label_Id")]
    public string Id { get; set; }
    /// <summary>
    /// Username
    /// </summary>
    [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
    [Display(ResourceType = typeof(Resources), Name = "Label_Username")]
    public string UserName { get; set; }
    /// <summary>
    /// User email
    /// </summary>
    [Required(ErrorMessageResourceName = "Error_Required", ErrorMessageResourceType = typeof(Resources))]
    [Display(ResourceType = typeof(Resources), Name = "Label_Email")]
    [EmailAddress]
    public string Email { get; set; }
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
