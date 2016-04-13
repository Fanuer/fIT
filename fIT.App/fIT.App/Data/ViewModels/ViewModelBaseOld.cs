using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Annotations;

namespace fIT.App.Data.ViewModels
{
  public class ViewModelBaseOld: INotifyPropertyChanged
  {
    #region FIELDS
    #endregion

    #region EVENTS
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion

    #region CTOR
    #endregion

    #region METHODS

    /// <summary>
    /// Sets a properties value and executes the PropertyChanged-Event
    /// Nutzung: public double Number { set { SetProperty(ref number, value); } get { return number; } }
    /// </summary>
    /// <typeparam name="T">Type of the Property</typeparam>
    /// <param name="storage">Private field with the value</param>
    /// <param name="value">New value</param>
    /// <param name="propertyName">Name of the Property that is changed. Need not be set, if it is called within a property</param>
    /// <returns></returns>
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
      if (Object.Equals(storage, value))
      {
        return false;
      }
      storage = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region PROPERTIES
    #endregion



  }
}
