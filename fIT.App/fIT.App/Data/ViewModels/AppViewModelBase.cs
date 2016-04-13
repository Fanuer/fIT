using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.Datamodels;
using fIT.App.Interfaces;
using GalaSoft.MvvmLight;

namespace fIT.App.Data.ViewModels
{
  public abstract class AppViewModelBase : ViewModelBase
  {
    #region FIELDS

    private IRepository _rep;
    #endregion

    #region CTOR

    protected AppViewModelBase(IRepository rep)
    {
      if (rep== null)
      {
        throw new ArgumentNullException(nameof(rep));
      }
      this._rep = rep;
    }
    #endregion

    #region METHODS

    public abstract Task InitAsync();

    #endregion

    #region PROPERTIES

    #endregion
  }
}
