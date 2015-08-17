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
using System.Threading.Tasks;

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
        private int scheduleId;

        protected async override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                SetContentView(Resource.Layout.ExerciseLayout);
                
                List<ExerciseModel> exercises = new List<ExerciseModel>();
                connectivityPointer = FindViewById<ImageView>(Resource.Id.ivConnectionExcercise);
                ListView lv = (ListView)FindViewById(Resource.Id.lvExercise);

                setConnectivityStatus(OnOffService.Online);

                //ScheduleID auslesen und damit die Übungen auslesen
                scheduleId = Intent.GetIntExtra("Schedule", 0);
                ooService = new OnOffService();
                exercises = await ooService.GetExercisesForSchedule(scheduleId);

                ExerciseListViewAdapter adapter = new ExerciseListViewAdapter(this, exercises, scheduleId);
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
            string selectedExerciseDescription = exercises[e.Position].Description.ToString();
            Console.WriteLine(scheduleId);

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