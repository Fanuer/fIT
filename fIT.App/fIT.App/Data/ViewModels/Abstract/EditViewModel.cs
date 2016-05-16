using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace fIT.App.Data.ViewModels.Abstract
{
    public abstract class EditViewModel : AppViewModelBase
    {
        #region CONST
        #endregion

        #region FIELDS
        #endregion

        #region CTOR

        protected EditViewModel(string title) : base(title)
        {
            this.OnCancelClickCommand = new Command(async () =>await this.ViewModelNavigation.PopAsPopUpAsync());
            this.OnOkClickCommand = new Command(async () => await OnOkClickedHandlerAsync());
        }
        #endregion

        #region METHODS

        private async Task OnOkClickedHandlerAsync()
        {
            var parent = await this.ViewModelNavigation.PopAsPopUpAsync();
            this.IsLoading = true;
            try
            {
                if (parent != null)
                {
                    await HandleOkClickedAsync(parent);
                }
                else
                {
                    throw new ArgumentException($"The ViewModel/BindingContext of the Parent Page must inherit from {nameof(AppViewModelBase)}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error on handling Edit: {e.Message}{Environment.NewLine}{e.StackTrace}");
                this.ShowMessage("Error occured");
            }
            this.IsLoading = false;
        }

        protected abstract Task HandleOkClickedAsync(ViewModelBase viewModel);
        #endregion

        #region PROPERTIES
        public ICommand OnOkClickCommand { get; protected set; }
        public ICommand OnCancelClickCommand { get; protected set; }
        #endregion

        
    }
}
