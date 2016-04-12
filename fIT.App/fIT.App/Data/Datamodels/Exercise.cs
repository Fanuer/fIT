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
      return string.Format("[Exercise: LocalId={0}, WasOffline={1}, Id={2}, Description={3}, Name={4}, Url={5}", LocalId, WasOffline, Id, Description, Name, Url);
    }

  }
}
