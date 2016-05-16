using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels;
using fIT.App.Helpers.Navigation.Attributes;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace fIT.App.Pages
{
    [RegisterViewModel(typeof(EditPracticeViewModel))]
    public partial class EditPracticePage : PopupPage
    {
        public EditPracticePage()
        {
            InitializeComponent();
        }
    }
}
