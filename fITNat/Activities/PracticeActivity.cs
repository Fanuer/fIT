using System;

using Android.App;
using Android.OS;
using Android.Widget;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fITNat.Services;

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
            btnSavePractice.Click += (object sender, EventArgs args) =>
            {
                try
                {
                    int scheduleId = 0; //über den Benutzer (aus der Session)
                    int exerciseId = 0; //über den Weg zu dem Training
                    var session = mgnService.actualSession();
                    if (session != null)
                    {
                        userId = session.CurrentUserId.ToString();
                    }
                    else
                    {
                        //UserId aus der Exercise holen
                        //userId = 
                    }
                    double weight = Double.Parse(txtWeight.Text);
                    int repetitions = Java.Lang.Integer.ParseInt(txtRepetitions.Text);
                    int numberOfRepetitions = Java.Lang.Integer.ParseInt(txtNumberOfRepetitions.Text);

                    ooService.createPractice(scheduleId, exerciseId, userId, new DateTime(), weight, repetitions, numberOfRepetitions).Wait();
                    //Zurück zu der Übungsseite
                    OnBackPressed();
                }
                catch (ServerException ex)
                {
                    Console.WriteLine("Login-Fehler(Server): " + ex.StackTrace);
                    CreatePracticeFail();
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Login-Fehler: " + exc.StackTrace);
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