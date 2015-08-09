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
using fIT.WebApi.Client.Data.Models.Exercise;
using fIT.WebApi.Client.Data.Models.Shared;
using fIT.WebApi.Client.Data.Models.Schedule;

namespace fITNat
{
    [Activity(Label = "fITNat")]
    public class ExerciseActivity : Activity
    {
        private List<ExerciseModel> exercises;
        private ListView lv;
        private ImageView connectivityPointer;
        private OnOffService ooService;
        private Guid userId;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                SetContentView(Resource.Layout.ExerciseLayout);
                //ScheduleID und UserID auslesen und in die versteckten Felder der �bungen legen
                int scheduleID = Integer.ParseInt(Intent.GetStringExtra("Schedule"));
                string text = Intent.GetStringExtra("UserID") ?? "";
                userId = text.Cast<Guid>().First();
                
                connectivityPointer = FindViewById<ImageView>(Resource.Id.ivConnectionExcercise);
                ListView lv = (ListView)FindViewById(Resource.Id.lvExercise);



                //Hier die Schedules des Benutzers abholen und in die Liste einf�gen
                //eine foreach muss drum, um �ber jede Exercise der Schedule zu iterieren
                var result = ooService.GetScheduleByIdAsync(scheduleID);
                ScheduleModel schedule = result.Result;
                var temp = ooService.GetExercisesForSchedule(scheduleID);
                List<ExerciseModel> scheduleExercises = temp.Result;
                
                ExerciseListViewAdapter adapter = new ExerciseListViewAdapter(this, scheduleExercises, userId);
                adapter.scheduleID = scheduleID;
                lv.Adapter = adapter;


                lv.ItemClick += lv_ItemClick;
            }
            catch(System.Exception exc)
            {
                Console.WriteLine("Fehler beim Erstellen der Exercise-�bersicht: " + exc.StackTrace);
            }
        }

        /// <summary>
        /// Clickevent auf ein Element des ListViews
        /// Geht zu dem ausgew�hlten Training
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Daraus die ID der �bung holen und diesen dann abfragen + redirect auf die passende Seite!
            string selectedExerciseName = exercises[e.Position].Name.ToString();
            int exerciseId = Integer.ParseInt(exercises[e.Position].Id.ToString());
            //ScheduleId aus dem versteckten Feld auslesen
            string scheduleId = "";//txtExerciseViewScheduleID
            string selectedExerciseDescription = exercises[e.Position].Description.ToString();

            var practiceActivity = new Intent(this, typeof(PracticeActivity));
            practiceActivity.PutExtra("Exercise", exerciseId);
            practiceActivity.PutExtra("Schedule", scheduleId);
            practiceActivity.PutExtra("UserID", userId.ToString());

            StartActivity(practiceActivity);
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