using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace fIT.App.Controls
{
    public class UnselectListView: ListView
    {
        #region CONST
        #endregion

        #region FIELDS
        #endregion

        #region CTOR

        public UnselectListView()
        {
            this.ItemSelected += (sender, args) =>
            {
                if (args.SelectedItem != null)
                {
                    this.SelectedItem = null;
                }
            };
        }
        #endregion

        #region METHODS
        #endregion

        #region PROPERTIES
        #endregion
    }
}
