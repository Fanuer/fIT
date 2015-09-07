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
using fIT.WebApi.Client.Data.Models.Practice;
using fIT.WebApi.Client.Data.Models.Exceptions;

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
        private int connectivity;
        private int scheduleId;
        private int exerciseId;
        private string userId;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                SetContentView(Resource.Layout.PracticeLayout);
                ooService = new OnOffService();

                connectivityPointer = FindViewById<ImageView>(Resource.Id.ivConnectionPractice);
                txtWeight = FindViewById<EditText>(Resource.Id.txtWeight);
                txtRepetitions = FindViewById<EditText>(Resource.Id.txtRepetitions);
                txtNumberOfRepetitions = FindViewById<EditText>(Resource.Id.txtNumberOfRepetitions);
                btnSavePractice = FindViewById<Button>(Resource.Id.btnSavePractice);
                setConnectivityStatus(OnOffService.Online);
                btnSavePractice.Click += bt_ItemClick;
            }
            catch(ArgumentNullException e)
            {
                Console.WriteLine("Fehler bei der Datenübertragung zwischen den Activities: " + e.StackTrace);
            }
            catch(Exception exc)
            {
                Console.WriteLine("Fehler in der PracticeActivity: " + exc.StackTrace);
            }
            
        }

        private async void bt_ItemClick(object sender, EventArgs e)
        {
            try
            {
                scheduleId = Intent.GetIntExtra("Schedule", 0);
                exerciseId = Intent.GetIntExtra("Exercise", 0);
                userId = Intent.GetStringExtra("User");
                double weight = Double.Parse(txtWeight.Text);
                int repetitions = Java.Lang.Integer.ParseInt(txtRepetitions.Text);
                int numberOfRepetitions = Java.Lang.Integer.ParseInt(txtNumberOfRepetitions.Text);

                bool result = await ooService.createPracticeAsync(scheduleId, exerciseId, userId, DateTime.Now, weight, repetitions, numberOfRepetitions);
                if(result)
                { 
                    //Zurück zu der Übungsseite
                    OnBackPressed();
                }
                else
                {
                    new AlertDialog.Builder(this)
                        .SetMessage("Anlegen ist schiefgegangen")
                        .SetTitle("Error")
                        .Show();
                }
            }
            catch (ServerException ex)
            {
                Console.WriteLine("Fehler beim Speichern des Trainings(Server): " + ex.StackTrace);
                CreatePracticeFail();
            }
            catch(FormatException exc)
            {
                Console.WriteLine("Fehler beim Parsen in der PracticeActivity: " + exc.StackTrace);
            }
            catch (Exception exce)
            {
                Console.WriteLine("Login-Fehler: " + exce.StackTrace);
            }
        }

        private void CreatePracticeFail()
        {
            new AlertDialog.Builder(this)
                        .SetMessage("Anlegen ist schiefgegangen")
                        .SetTitle("Error")
                        .Show();
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
            connectivityPointer.SetImageResource(0);
            if (online)
                connectivity = Resource.Drawable.CheckDouble;
            else
                connectivity = Resource.Drawable.Check;
            connectivityPointer.SetImageResource(connectivity);
        }
    }
}