using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace fIT.App.Data.Datamodels
{
  [Table("Exercise")]
  public class Exercise :LocalDataModelBase
  {
    public int Id { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public override string ToString()
    {
      return $"[Exercise: LocalId={LocalId}, WasOffline={WasOffline}, Id={Id}, Description={Description}, Name={Name}";
    }

  }
}
