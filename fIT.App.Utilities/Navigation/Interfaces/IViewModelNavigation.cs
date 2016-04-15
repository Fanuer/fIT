using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace fIT.App.Utilities.Navigation.Interfaces
{
  public interface IViewModelNavigation
  {
    Task<ViewModelBase> PopAsync();

    Task<ViewModelBase> PopModalAsync();

    Task PopToRootAsync();

    Task PushAsync(ViewModelBase viewModel);

    Task PushModalAsync(ViewModelBase viewModel);

    Task<ViewModelBase> ExchangeAync(ViewModelBase viewModel);
  }
}