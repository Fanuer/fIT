using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Utilities.Navigation.Attributes;
using fIT.App.Utilities.Navigation.Interfaces;
using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace fIT.App.Utilities.Navigation
{
  public class NavigationFrame : IViewModelNavigation
  {
    #region FIELDS
    private static readonly Dictionary<Type, Type> ViewModelTypeToPageType;

    private readonly NavigationPage _navigationPage;
    #endregion

    #region CTOR
    static NavigationFrame()
    {
      ViewModelTypeToPageType = new Dictionary<Type, Type>();

      // This is the hacky way we have to get the list of assemblies in a PCL for now.
      // Hopefully Xamarin will expose Device.GetAssemblies() in a future version of Xamarin.Forms.


      var allTypes = IoCLocator.Current.DomainTypes;
      var typesWithRegisterAttributes = allTypes
          .Select(t => new { TypeInfo = t, Attribute = t.GetCustomAttribute<RegisterViewModelAttribute>() })
          .Where(p => p.Attribute != null);
      foreach (var pair in typesWithRegisterAttributes)
      {
        if (!typeof(Page).GetTypeInfo().IsAssignableFrom(pair.TypeInfo))
        {
          var message = $"RegisterViewModelAttribute applied to a class ({pair.TypeInfo.FullName}) that is not a Page";
          throw new InvalidOperationException(message);
        }
        if (ViewModelTypeToPageType.ContainsKey(pair.Attribute.ViewModelType))
        {
          var message = $"Multiple Page types (new = {pair.TypeInfo.FullName}, previous = {ViewModelTypeToPageType[pair.Attribute.ViewModelType].FullName}) registered for the same view model type ({pair.Attribute.ViewModelType.FullName})";
          throw new InvalidOperationException(message);
        }
        ViewModelTypeToPageType[pair.Attribute.ViewModelType] = pair.TypeInfo.AsType();
      }
    }

    public NavigationFrame(ViewModelBase rootViewModel)
    {
      _navigationPage = new NavigationPage(CreatePageForViewModel(rootViewModel));
    }
    #endregion

    #region METHODS
    public async Task<ViewModelBase> PopAsync()
    {
      var currentViewModel = CurrentViewModel;
      await _navigationPage.PopAsync();
      return currentViewModel;
    }

    public async Task<ViewModelBase> PopModalAsync()
    {
      var poppedPage = await _navigationPage.Navigation.PopModalAsync();
      return (ViewModelBase)poppedPage.BindingContext;
    }

    public async Task PopToRootAsync()
    {
      await _navigationPage.PopToRootAsync();
    }

    public async Task PushAsync(ViewModelBase viewModel)
    {
      await _navigationPage.PushAsync(CreatePageForViewModel(viewModel));
    }

    public Task PushModalAsync(ViewModelBase viewModel)
    {
      return _navigationPage.Navigation.PushModalAsync(CreatePageForViewModel(viewModel));
    }

    public async Task<ViewModelBase> ExchangeAync(ViewModelBase viewModel)
    {
      var newPage = CreatePageForViewModel(viewModel);
      _navigationPage.Navigation.InsertPageBefore(newPage, _navigationPage.CurrentPage);
      return await this.PopAsync();
    }

    private Page CreatePageForViewModel(ViewModelBase viewModel)
    {
      if (viewModel == null)
      {
        throw new ArgumentNullException(nameof(viewModel));
      }

      Type newPageType = null;
      if (!ViewModelTypeToPageType.TryGetValue(viewModel.GetType(), out newPageType))
      {
        throw new ArgumentException("Trying to create a Page for an unrecognized view model type. Did you forget to use the RegisterViewModel attribute?");
      }
      Page newPage = null;
      try
      {
        newPage = (Page)Activator.CreateInstance(ViewModelTypeToPageType[viewModel.GetType()]);
        ;
      }
      catch (Exception e)
      {
        throw e;
      }
      
      newPage.BindingContext = viewModel;
      var navigatedPage = viewModel as INavigatingViewModel;
      if (navigatedPage != null)
      {
        navigatedPage.ViewModelNavigation = this;
      }

      return newPage;
    }

    #endregion

    #region PROPERTIES
    public Page Root { get { return _navigationPage; } }

    public ViewModelBase CurrentViewModel => _navigationPage.CurrentPage.BindingContext as ViewModelBase;

    #endregion
  }
}
