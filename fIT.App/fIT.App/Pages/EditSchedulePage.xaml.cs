using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels;
using fIT.App.Utilities.Navigation.Attributes;
using Xamarin.Forms;

namespace fIT.App.Pages
{
    [RegisterViewModel(typeof(EditScheduleViewModel))]
    public partial class EditSchedulePage : BasePage
    {
        public EditSchedulePage()
        {
            InitializeComponent();
        }
    }
}
