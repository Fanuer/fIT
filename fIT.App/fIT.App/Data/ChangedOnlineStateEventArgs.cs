using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.App.Data
{
  /// <summary>
  /// Event Arguments that show, the current Connection Status
  /// </summary>
  public class ChangedOnlineStateEventArgs:EventArgs
  {
    #region FIELDS
    #endregion

    #region CTOR

    public ChangedOnlineStateEventArgs(bool status)
    {
      this.Status = status;
    }
    #endregion

    #region METHODS
    #endregion

    #region PROPERTIES
    /// <summary>
    /// Current Connectivity Status to the Server
    /// </summary>
    public bool Status { get; set; }
    #endregion

  }
}
