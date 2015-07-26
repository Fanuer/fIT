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
    [Activity(Label = "fITNat")]
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

            //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.ScheduleView, Resource.Id.txtScheduleViewDescription, schedules);
            ScheduleListViewAdapter adapter = new ScheduleListViewAdapter(this, schedules);
            ListView lv = (ListView)FindViewById(Resource.Id.lvSchedules);
            lv.Adapter = adapter;


            lv.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                String selectedFromList = lv.GetItemAtPosition(e.Position).ToString();
                //Daraus die ID des Trainingsplans holen und diesen dann abfragen + redirect auf die passende Seite!
            };
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}