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

namespace fITNat
{
    [Activity(Label = "fITNat")]
    public class ExerciseActivity : Activity
    {
        private List<ExerciseModel> exercises;
        private ListView lv;
        private ImageView connectivityPointer;
        private OnOffService ooService;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                SetContentView(Resource.Layout.ExerciseLayout);
                //ScheduleID auslesen und in die versteckten Felder der Übungen legen
                int scheduleID = Integer.ParseInt(Intent.GetStringExtra("Schedule"));

                List<ExerciseModel> exercises = new List<ExerciseModel>();
                connectivityPointer = FindViewById<ImageView>(Resource.Id.ivConnectionExcercise);
                ListView lv = (ListView)FindViewById(Resource.Id.lvExercise);

                //Hier die Schedules des Benutzers abholen und in die Liste einfügen
                //eine foreach muss drum, um über jede Exercise in der Schedule zu iterieren
                var schedule = ooService.GetScheduleByIdAsync(scheduleID);
                IEnumerable<EntryModel<int>> scheduleExercises = schedule.Result.Exercises;

                foreach (var item in scheduleExercises)
                {
                    var task = ooService.GetExerciseByIdAsync(item.Id);
                    exercises.Add(task.Result);
                }


                ExerciseListViewAdapter adapter = new ExerciseListViewAdapter(this, exercises);
                adapter.scheduleID = scheduleID;
                lv.Adapter = adapter;


                lv.ItemClick += lv_ItemClick;
            }
            catch(System.Exception exc)
            {
                Console.WriteLine("Fehler beim Erstellen der Exercise-Übersicht: " + exc.StackTrace);
            }
        }

        /// <summary>
        /// Clickevent auf ein Element des ListViews
        /// Geht zu dem ausgewählten Training
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Daraus die ID der Übung holen und diesen dann abfragen + redirect auf die passende Seite!
            string selectedExerciseName = exercises[e.Position].Name.ToString();
            int exerciseId = Integer.ParseInt(exercises[e.Position].Id.ToString());
            //ScheduleId aus dem versteckten Feld auslesen
            string scheduleId = "";//txtExerciseViewScheduleID
            string selectedExerciseDescription = exercises[e.Position].Description.ToString();

            var practiceActivity = new Intent(this, typeof(PracticeActivity));
            practiceActivity.PutExtra("Exercise", exerciseId);
            practiceActivity.PutExtra("Schedule", scheduleId);
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