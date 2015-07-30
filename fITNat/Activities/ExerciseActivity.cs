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
    public class ExerciseActivity : Activity
    {
        private List<Exercise> exercises;
        private ListView lv;
        private ManagementServiceLocal mgnService;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ExerciseLayout);

            List<Exercise> exercises = new List<Exercise>();
            ListView lv = (ListView)FindViewById(Resource.Id.lvExercise);

            //Generiert Testdaten
            /*
            for (int i = 0; i < 10; i++)
            {
                exercises.Add(i + ". Exercise");
            }
            */
            //Hier die Schedules des Benutzers abholen und in die Liste einf�gen
            //await mgnService


            //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Resource.Layout.ScheduleView, Resource.Id.txtScheduleViewDescription, schedules);
            ExerciseListViewAdapter adapter = new ExerciseListViewAdapter(this, exercises);
            lv.Adapter = adapter;


            lv.ItemClick += lv_ItemClick;
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
            int tid = Integer.ParseInt(exercises[e.Position].Id.ToString());
            string selectedExerciseDescription = exercises[e.Position].Description.ToString();
            //Exercise exercise = new Exercise(tid, selectedExerciseName, selectedExerciseDescription, session.ID);
            //exercise.FindSingle(tid);

            SetContentView(Resource.Layout.ExerciseLayout);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}