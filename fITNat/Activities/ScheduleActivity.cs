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
        private List<Schedule> schedules;
        private ListView lv;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ScheduleLayout);

            schedules = new List<Schedule>();
            lv = (ListView)FindViewById(Resource.Id.lvSchedules);

            for (int i = 1; i <= 10; i++)
            {
                schedules.Add(new Schedule(i, "Testplan "+i, "Kevin"));
            }

            //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.ScheduleView, Resource.Id.txtScheduleViewDescription, schedules);
            ScheduleListViewAdapter adapter = new ScheduleListViewAdapter(this, schedules);
            lv.Adapter = adapter;


            lv.ItemClick += lv_ItemClick;
        }

        /// <summary>
        /// Clickevent auf ein Element des ListViews
        /// Geht zu dem ausgewählten Trainingsplan und zeigt dort die Übungen an
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            String selectedFromList = schedules[e.Position].ToString();
            //Daraus die ID des Trainingsplans holen und diesen dann abfragen + redirect auf die passende Seite!
            //int id = schedules[e.Position].id;
            //Schedule schedule = new Schedule();
            //schedule.FindSingle(id);

            SetContentView(Resource.Layout.ExerciseLayout);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}