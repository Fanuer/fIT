using System;
using fIT.WebApi.Client.Models.Shared.Enums;

namespace fIT.WebApi.Client.Models.Account
{
  /// <summary>
  /// User data
  /// </summary>
  public class UserModel
  {
    /// <summary>
    /// Url to receive this object
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// User Id
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Username
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; set; }
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
    /// Geburtstag
    /// </summary>
    public DateTime DateOfBirth { get; set; }
  }
}
