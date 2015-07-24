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
    [Activity(Label = "OverviewActivity")]
    public class ScheduleActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ScheduleLayout);

            List<string> schedules = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                schedules.Add("value: " + i);
            }

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.ScheduleView, Resource.Id.txtScheduleViewDescription, schedules);
            ListView lv = (ListView)FindViewById(Resource.Id.lvSchedules);
            lv.Adapter = adapter;
        }
    }
}