using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace fIT.App.Utilities.Navigation.Attributes
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
