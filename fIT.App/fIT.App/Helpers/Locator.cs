using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels;
using fIT.App.Interfaces;
using fIT.App.Repositories;
using fIT.App.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace fIT.App.Helpers
{
  public class Locator
  {
    #region FIELDS
    #endregion

    #region CTOR
    #endregion

    #region METHODS
    /// <summary>
    /// Register all the used ViewModels, Services et. al. witht the IoC Container
    /// </summary>
    public Locator()
    {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

      // ViewModels
      SimpleIoc.Default.Register<ScheduleViewModel>();

      // Services
      SimpleIoc.Default.Register<IRepository, Respoitory>();
    }
    #endregion

    #region PROPERTIES
    /// <summary>
    /// Gets the Main property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
    public ScheduleViewModel Main
    {
      get { return ServiceLocator.Current.GetInstance<ScheduleViewModel>(); }
    }
    #endregion

  }
}
