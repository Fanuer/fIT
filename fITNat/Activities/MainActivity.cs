using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.Threading;
using fITNat.Services;

namespace fITNat
{
    [Activity(Label = "fITNat", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button mBtnSignUp;
        private Button mBtnSignIn;
        private ProgressBar progressBar;
        private ImageView connectivityPointer;
        private Guid userId;
        private int connectivity;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Services starten
            ThreadPool.QueueUserWorkItem(o => StartService(new Intent(this, typeof(OnOffService))));
            ThreadPool.QueueUserWorkItem(o => StartService(new Intent(this, typeof(ManagementServiceLocal))));

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            mBtnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            mBtnSignIn = FindViewById<Button>(Resource.Id.btnSignIn);
            //progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            connectivityPointer = FindViewById<ImageView>(Resource.Id.ivConnection);
            //ConnectivityPointer belegen
            setConnectivityStatus(OnOffService.Online);

            mBtnSignUp.Click += (object sender, EventArgs args) =>
                {
                    //Pull up SignUp-dialog
                    FragmentTransaction transaction = FragmentManager.BeginTransaction();
                    Dialog_SignUp signUpDialog = new Dialog_SignUp();
                    signUpDialog.Show(transaction, "dialog fragment");

                    signUpDialog.onSignUpComplete += SignUpDialog_onSignUpComplete;
                };
            mBtnSignIn.Click += (object sender, EventArgs args) =>
            {
                //Pull up SignIn-dialog
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                Dialog_SignIn signInDialog = new Dialog_SignIn();
                signInDialog.Show(transaction, "dialog fragment");

                signInDialog.onSignInComplete += SignInDialog_onSignInComplete;
            };
        }

        private void SignUpDialog_onSignUpComplete(object sender, OnSignUpEventArgs e)
        {
            //ConnectivityPointer belegen
            setConnectivityStatus(OnOffService.Online);

            var intent = new Intent(this, typeof(ScheduleActivity));
            StartActivity(intent);

            //Anhand der InnerException heruasfinden was die Probleme gemacht hat
        }

        private void SignInDialog_onSignInComplete(object sender, OnSignInEventArgs e)
        {
            //ConnectivityPointer belegen
            setConnectivityStatus(OnOffService.Online);

            var intent = new Intent(this, typeof(ScheduleActivity));
            //Userinformationen mit übergeben
            userId = e.UserId;
            intent.PutExtra("User", userId.ToString());
            StartActivity(intent);
        }

        /// <summary>
        /// Belegt das Connectivity-Icon entsprechend des Verbindungsstatus
        /// </summary>
        /// <param name="online"></param>
        public void setConnectivityStatus(bool online)
        {
            if(online)
                connectivity = Resource.Drawable.CheckDouble;
            else
                connectivity = Resource.Drawable.Check;
            connectivityPointer.SetBackgroundResource(connectivity);
        }
    }
}

