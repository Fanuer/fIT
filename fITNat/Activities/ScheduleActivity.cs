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
using fITNat.Services;
using Java.Lang;

namespace fITNat
{
    [Activity(Label = "fITNat")]
    public class ScheduleActivity : Activity
    {
        private List<Schedule> schedules; //wurde kopiert, um Zugriff zu haben
        private ListView lv;
        private ManagementServiceLocal mgnService;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ScheduleLayout);

            schedules = new List<Schedule>();
            lv = (ListView)FindViewById(Resource.Id.lvSchedule);


            //Generiert Testdaten
            /*
            for (int i = 1; i <= 10; i++)
            {
                schedules.Add(new Schedule(i, "Testplan "+i, "Kevin"));
            }
            */
            //Hier die Schedules des Benutzers abholen und in die Liste einf�gen
            //await mgnService


            //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.ScheduleView, Resource.Id.txtScheduleViewDescription, schedules);
            ScheduleListViewAdapter adapter = new ScheduleListViewAdapter(this, schedules);
            lv.Adapter = adapter;


            lv.ItemClick += lv_ItemClick;
        }

        /// <summary>
        /// Clickevent auf ein Element des ListViews
        /// Geht zu dem ausgew�hlten Trainingsplan und zeigt dort die �bungen an
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Daraus die ID des Trainingsplans holen und diesen dann abfragen + redirect auf die passende Seite!
            string selectedSchedule = schedules[e.Position].Name.ToString();
            int tid = Integer.ParseInt(schedules[e.Position].Id.ToString());
            //Schedule schedule = new Schedule(tid, selectedSchedule, session.ID);
            //schedule.FindSingle(id);

            SetContentView(Resource.Layout.ExerciseLayout);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}