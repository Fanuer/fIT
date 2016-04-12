using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.App.Data.Exceptions
{
  public class InvalidForOfflineException:Exception
  {
    public InvalidForOfflineException():base("This Action is invalid in Offline-Context. Please try again, when the Server is available again")
    {
    }
  }
}
