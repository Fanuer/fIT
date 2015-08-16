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

namespace fITNat
{
    [Activity(Label = "fITNat")]
    public class ExerciseActivity : Activity
    {
        private List<ExerciseModel> exercises;
        private ListView lv;
        private ImageView connectivityPointer;
        private OnOffService ooService;
        private int connectivity;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ExerciseLayout);
            //ScheduleID auslesen und in die versteckten Felder der Übungen legen
            string schedule = Intent.GetStringExtra("Schedule");

            List<ExerciseModel> exercises = new List<ExerciseModel>();
            connectivityPointer = FindViewById<ImageView>(Resource.Id.ivConnectionExcercise);
            ListView lv = (ListView)FindViewById(Resource.Id.lvExercise);

            setConnectivityStatus(OnOffService.Online);
            //Hier die Schedules des Benutzers abholen und in die Liste einfügen
            //eine foreach muss drum, um über jede Exercise in der Schedule zu iterieren
            //var task = ooService.GetExerciseByIdAsync();
            //exercises = task.Result.ToList();


            ExerciseListViewAdapter adapter = new ExerciseListViewAdapter(this, exercises);
            adapter.scheduleID = schedule;
            lv.Adapter = adapter;


            lv.ItemClick += lv_ItemClick;
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
                connectivity = Resource.Drawable.CheckDouble;
            else
                connectivity = Resource.Drawable.Check;
            connectivityPointer.SetBackgroundResource(connectivity);
        }
    }
}