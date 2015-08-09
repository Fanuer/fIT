using System;

using Android.App;
using Android.OS;
using Android.Widget;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fITNat.Services;
using Java.Lang;

namespace fITNat
{
    [Activity(Label = "fITNat")]
    public class PracticeActivity : Activity
    {
        private ImageView connectivityPointer;
        private EditText txtWeight;
        private EditText txtRepetitions;
        private EditText txtNumberOfRepetitions;
        private Button btnSavePractice;
        private OnOffService ooService;
        private ManagementServiceLocal mgnService;
        private string userId;
        private int exerciseId;
        private int scheduleId;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.PracticeLayout);
            mgnService = new ManagementServiceLocal();

            connectivityPointer = FindViewById<ImageView>(Resource.Id.ivConnectionPractice);
            txtWeight = FindViewById<EditText>(Resource.Id.txtWeight);
            txtRepetitions = FindViewById<EditText>(Resource.Id.txtRepetitions);
            txtNumberOfRepetitions = FindViewById<EditText>(Resource.Id.txtNumberOfRepetitions);
            btnSavePractice = FindViewById<Button>(Resource.Id.btnSavePractice);

            //Infos aus dem Aufruf holen
            scheduleId = Integer.ParseInt(Intent.GetStringExtra("Schedule"));
            exerciseId = Integer.ParseInt(Intent.GetStringExtra("Exercise"));

            btnSavePractice.Click += (object sender, EventArgs args) =>
            {
                try
                {
                    var session = mgnService.actualSession();
                    if (session != null)
                    {
                        userId = session.CurrentUserId.ToString();
                    }
                    else
                    {
                        //UserId aus der Exercise holen
                        userId = Intent.GetStringExtra("UserID") ?? "";
                    }
                    double weight = System.Double.Parse(txtWeight.Text);
                    int repetitions = Java.Lang.Integer.ParseInt(txtRepetitions.Text);
                    int numberOfRepetitions = Java.Lang.Integer.ParseInt(txtNumberOfRepetitions.Text);

                    var result = ooService.createPractice(scheduleId, exerciseId, userId, new DateTime(), weight, repetitions, numberOfRepetitions);
                    bool creation = result.Result;
                    if(creation)
                    {
                        new AlertDialog.Builder(this)
                           .SetMessage("Speichern erfolgreich!")
                           .Show();
                        //Zurück zu der Übungsseite
                        OnBackPressed();
                    }
                    else
                    {
                        //Messagebox anzeigen, die über den Fehler informiert
                        new AlertDialog.Builder(this)
                           .SetMessage("Fehler beim Anlegen des Trainings")
                           .Show();
                        OnBackPressed();
                    }
                }
                catch (ServerException ex)
                {
                    Console.WriteLine("Fehler beim Anlegen eines Trainings (Server): " + ex.StackTrace);
                    CreatePracticeFail();
                }
                catch (System.Exception exc)
                {
                    Console.WriteLine("Fehler beim Anlegen eines Trainings: " + exc.StackTrace);
                }
            };
        }

        private void CreatePracticeFail()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Beim Drücken des Zurück-Knopfs
        /// </summary>
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