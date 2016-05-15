using System;
using System.Reflection;
using GalaSoft.MvvmLight;

namespace fIT.App.Helpers.Navigation.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RegisterViewModelAttribute : Attribute
  {
    public Type ViewModelType { get; private set; }

    public RegisterViewModelAttribute(Type viewModelType)
    {
      var baseType = typeof (ViewModelBase);

      if (!viewModelType.GetTypeInfo().IsSubclassOf(baseType))
      {
        throw new ArgumentException($"ViewModel must inherit from {baseType.FullName}");
      }
      ViewModelType = viewModelType;
    }
  }
}
