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
using fITNat.Activities;

namespace fITNat
{
    [Activity(Label = "fITNat")]
    public class ExerciseActivity : Activity
    {
        public static List<ExerciseModel> exercises { get; private set; }
        private ListView lv;
        private ImageView connectivityPointer;
        private OnOffService ooService;
        private int connectivity;
        private int scheduleId;
        private string userId;

        public static void SetExercises(List<ExerciseModel> data)
        {
            exercises = data;
        }

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
                userId = Intent.GetStringExtra("User");
                ooService = new OnOffService();
                exercises = await ooService.GetExercisesForSchedule(scheduleId);
                if(exercises != null)
                {
                    SetExercises(exercises);

                    ExerciseListViewAdapter adapter = new ExerciseListViewAdapter(this, exercises, scheduleId);
                    lv.Adapter = adapter;

                    lv.ItemClick += lv_ItemClick;
                    lv.ItemLongClick += lv_ItemLongClick;
                }
                else
                {
                    new AlertDialog.Builder(this)
                            .SetMessage("Keine Übungen vorhanden")
                            .SetTitle("Warning")
                            .Show();
                    OnBackPressed();
                }
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
        /// Clickevent bei langem Drücken auf ein Element des ListViews, um die Statistik anzuzeigen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lv_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            int exerciseId = Integer.ParseInt(exercises[e.Position].Id.ToString());
            var statisticActivity = new Intent(this, typeof(StatisticActivity));
            statisticActivity.PutExtra("Exercise", exerciseId);
            statisticActivity.PutExtra("Schedule", scheduleId);
            statisticActivity.PutExtra("User", userId);
            StartActivity(statisticActivity);
        }


        /// <summary>
        /// Clickevent auf ein Element des ListViews
        /// Geht zu dem ausgewählten Training
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string selectedExerciseName = exercises[e.Position].Name.ToString();
            int exerciseId = Integer.ParseInt(exercises[e.Position].Id.ToString());
            string selectedExerciseDescription = exercises[e.Position].Description.ToString();

            var practiceActivity = new Intent(this, typeof(PracticeActivity));
            practiceActivity.PutExtra("Exercise", exerciseId);
            practiceActivity.PutExtra("Schedule", scheduleId);
            practiceActivity.PutExtra("User", userId);
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