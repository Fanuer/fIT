using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;

namespace fITNat
{
    [Activity(Label = "fITNat", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button mBtnSignUp;
        private Button mBtnSignIn;
        private ProgressBar progressBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            mBtnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            mBtnSignIn = FindViewById<Button>(Resource.Id.btnSignIn);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            mBtnSignUp.Click += (object sender, EventArgs args) =>
                {
                    //Pull up SignUp-dialog
                    FragmentTransaction transaction = FragmentManager.BeginTransaction();
                    dialog_SignUp signUpDialog = new dialog_SignUp();
                    signUpDialog.Show(transaction, "dialog fragment");

                    signUpDialog.onSignUpComplete += SignUpDialog_onSignUpComplete;
                };
            mBtnSignIn.Click += (object sender, EventArgs args) =>
            {
                //Pull up SignIn-dialog
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                dialog_SignIn signInDialog = new dialog_SignIn();
                signInDialog.Show(transaction, "dialog fragment");

                signInDialog.onSignInComplete += SignInDialog_onSignInComplete;
            };
        }

        private void SignUpDialog_onSignUpComplete(object sender, OnSignUpEventArgs e)
        {
            //Show the Loader
            progressBar.Visibility = ViewStates.Visible;

            //Data of Registration to send them to the server
            Thread thread = new Thread(actLikeARequest);
            thread.Start();
            /*
            string username = e.Username;
            string email = e.Email;
            string password = e.Password;
            string birthdate = e.Birthdate;
            */
        }

        private void SignInDialog_onSignInComplete(object sender, OnSignInEventArgs e)
        {
            //Show the Loader
            progressBar.Visibility = ViewStates.Visible;

            //Data of Registration to send them to the server
            Thread thread = new Thread(actLikeARequest);
            thread.Start();
            /*
            string username = e.Username;
            string email = e.Email;
            string password = e.Password;
            string birthdate = e.Birthdate;
            */
        }

        private void actLikeARequest()
        {
            Thread.Sleep(3000);

            //Hide the Loader with the Main-Thread
            RunOnUiThread(() => { progressBar.Visibility = ViewStates.Invisible; });
        }
    }
}

