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
using fIT.WebApi.Client.Data.Models.Schedule;
using System.Threading.Tasks;

namespace fITNat
{
    [Activity(Label = "fITNat")]
    public class ScheduleActivity : Activity
    {
        //ScheduleModel kommt noch von Stefan
        private List<ScheduleModel> schedules;
        private ListView lv;
        private ImageView connectivityPointer;
        private OnOffService ooService;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ScheduleLayout);

            schedules = new List<ScheduleModel>();
            lv = (ListView)FindViewById(Resource.Id.lvSchedule);
            connectivityPointer = FindViewById<ImageView>(Resource.Id.ivConnectionSchedule);


            //Generiert Testdaten
            /*
            for (int i = 1; i <= 10; i++)
            {
                schedules.Add(new Schedule(i, "Testplan "+i, "Kevin"));
            }
            */
            //Hier die Schedules des Benutzers abholen und in die Liste einfügen
            var task = ooService.GetAllSchedulesAsync();
            schedules = task.Result.ToList();

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
            //Daraus die ID des Trainingsplans holen und diesen dann abfragen + redirect auf die passende Seite!
            string selectedSchedule = schedules[e.Position].Name.ToString();
            int scheduleId = Integer.ParseInt(schedules[e.Position].Id.ToString());

            var exerciseActivity = new Intent(this, typeof(ExerciseActivity));
            exerciseActivity.PutExtra("Schedule", scheduleId);
            StartActivity(exerciseActivity);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }


        /// <summary>
        /// Belegt das Connectivity-Icon entsprechend des Verbindungsstatus
        /// </summary>
        /// <param name="online"></param>
        public void setConnectivityStatus(bool online)
        {
            if (online)
                connectivityPointer.SetBackgroundResource(Resource.Drawable.CheckDouble);
            else
                connectivityPointer.SetBackgroundResource(Resource.Drawable.Check);
        }
    }
}