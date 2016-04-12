using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace fIT.App.Data.Datamodels
{
  public class LocalDataModelBase
  {
    [PrimaryKey, AutoIncrement]
    public int LocalId { get; set; }

    public bool WasOffline { get; set; }
  }
}
