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
            //Services starten
            StartService(new Intent(this, typeof(OnOffService)));
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
            progressBar.Visibility = ViewStates.Visible;

            string username = e.Username;
            string email = e.Email;
            string password = e.Password;
            string passwordConfirm = e.PasswordConfirm;
            string gender = e.Gender;
            string job = e.Job;
            string fitness = e.Fitness;
            DateTime birthdate = e.Birthdate;

            //Dictionary für JSON aufbauen
            Dictionary<string, string> userSignUp = new Dictionary<string, string>();
            userSignUp.Add("username", username);
            userSignUp.Add("email", email);
            userSignUp.Add("password", password);
            userSignUp.Add("confirmPassword", passwordConfirm);
            userSignUp.Add("gender", gender.ToString());
            userSignUp.Add("job", job);
            userSignUp.Add("fitness", fitness);
            userSignUp.Add("dateOfBirth", birthdate.ToString());
            //OnOffService.decideSignUp(userSignUp);

            Thread thread = new Thread(doSomething);
            thread.Start();
            thread.Join();
            //Hide the Loader
            progressBar.Visibility = ViewStates.Invisible;
            var intent = new Intent(this, typeof(ScheduleActivity));
            StartActivity(intent);


            /*
            Testaufruf der REST-API  --> Fehlerhaft!!
            string url = "http://fit-bachelor.azurewebsites.net:80/api/Accounts/Register";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "POST";
            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = userRegister.ToString();
                    Console.WriteLine(json);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.Write(result);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            */
        }

        private void SignInDialog_onSignInComplete(object sender, OnSignInEventArgs e)
        {
            //Show the Loader
            progressBar.Visibility = ViewStates.Visible;

            var intent = new Intent(this, typeof(ScheduleActivity));
            StartActivity(intent);

            //Data of Registration to send them to the server
            string username = e.Username;
            string password = e.Password;

            Dictionary<string, string> userSignIn = new Dictionary<string, string>();
            userSignIn.Add("username", username);
            userSignIn.Add("password", password);
            //Service-Aufruf
            //OnOffService.decideSignIn(userSignIn);
        }

        private void doSomething()
        {
            Thread.Sleep(3000);
        }
    }
}

