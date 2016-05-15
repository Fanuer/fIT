using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.App.Data.ViewModels;
using fIT.App.Helpers.Navigation.Attributes;
using Xamarin.Forms;

namespace fIT.App.Pages
{
    [RegisterViewModel(typeof(ScheduleViewModel))]
    public partial class SchedulePage : BasePage
    {
        public SchedulePage()
        {
            InitializeComponent();
        }
    }
}
