using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Shared.Enums;
using SQLite.Net.Attributes;

namespace fIT.App.Data.Datamodels
{
  [Table("User")]
  public class User: LocalDataModelBase
  {
    [PrimaryKey]
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
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
    public override string ToString()
    {
      return $"[User: LocalId={LocalId}, UserId={UserId}, Username={Username}, Password={Password}]";
    }
  }
}
