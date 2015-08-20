using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;
using fIT.WebApi.Client.Data.Models.Schedule;

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
        private int connectivity;
        private string userID;

        protected async override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                SetContentView(Resource.Layout.ScheduleLayout);

                schedules = new List<ScheduleModel>();
                lv = (ListView)FindViewById(Resource.Id.lvSchedule);
                connectivityPointer = FindViewById<ImageView>(Resource.Id.ivConnectionSchedule);
                setConnectivityStatus(OnOffService.Online);

                //Hier die Schedules des Benutzers abholen und in die Liste einfügen
                ooService = new OnOffService();
                userID = Intent.GetStringExtra("User");
                Guid userId = new Guid(userID);
                IEnumerable<ScheduleModel> temp = await ooService.GetAllSchedulesAsync(userId);
                schedules = temp.ToList<ScheduleModel>();

                //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.ScheduleView, Resource.Id.txtScheduleViewDescription, schedules);
                ScheduleListViewAdapter adapter = new ScheduleListViewAdapter(this, schedules);
                lv.Adapter = adapter;


                lv.ItemClick += lv_ItemClick;
            }
            catch(ArgumentNullException ex)
            {
                Console.WriteLine("Keine UserId übergeben: " + ex.StackTrace);
            }
            catch(System.Exception exc)
            {
                Console.WriteLine("Fehler beim Erstellen des ScheduleViews: " + exc.StackTrace);
            }
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
            exerciseActivity.PutExtra("User", userID);
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
                connectivity = Resource.Drawable.CheckDouble;
            else
                connectivity = Resource.Drawable.Check;
            connectivityPointer.SetBackgroundResource(0);
            connectivityPointer.SetBackgroundResource(connectivity);
        }
    }
}