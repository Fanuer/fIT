using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace fITNat
{
    [Activity(Label = "PracticeActivity")]
    public class PracticeActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.PracticeLayout);

            List<string> practices = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                practices.Add(i + ". Practice");
            }

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.PracticeView, Resource.Id.txtPracticeViewDescription, practices);
            ListView lv = (ListView)FindViewById(Resource.Id.lvPractice);
            lv.Adapter = adapter;
        }
    }
}