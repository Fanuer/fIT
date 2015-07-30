using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Android.Graphics.Drawables;
using fIT.WebApi.Client.Data;
using fIT.WebApi.Client.Portable.Implementation;
using fITNat.Services;
using fIT.WebApi.Client.Data.Models.Shared.Enums;

namespace fITNat
{
    [Activity(Label = "fITNat", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button mBtnSignUp;
        private Button mBtnSignIn;
        private ProgressBar progressBar;
        private ManagementServiceLocal mngService;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Services starten
            //StartService(new Intent(this, typeof(OnOffService)));
            StartService(new Intent(this, typeof(ManagementServiceLocal)));

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            mBtnSignUp = FindViewById<Button>(Resource.Id.btnSignUp);
            mBtnSignIn = FindViewById<Button>(Resource.Id.btnSignIn);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
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
            //Show the Loader
            //progressBar.Visibility = ViewStates.Visible;
            //Hide the Loader
            //progressBar.Visibility = ViewStates.Invisible;

            var intent = new Intent(this, typeof(ScheduleActivity));
            StartActivity(intent);
        }

        private void SignInDialog_onSignInComplete(object sender, OnSignInEventArgs e)
        {
            //Show the Loader
            //progressBar.Visibility = ViewStates.Visible;

            var intent = new Intent(this, typeof(ScheduleActivity));
            StartActivity(intent);
        }
    }
}

