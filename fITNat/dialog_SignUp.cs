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
using fIT.WebApi.Client.Models.Shared.Enums;

namespace fITNat
{
    class dialog_SignUp : DialogFragment
    {
        private EditText txtUsername;
        private EditText txtEmail;
        private EditText txtPassword;
        private EditText txtPasswordConfirm;
        private Spinner spinGender;
        private Spinner spinJob;
        private Spinner spinFitness;
        private EditText txtBirthdate;
        private Button btnSignUp;
        private List<GenderType> genderItems;
        private List<JobTypes> jobItems;
        private List<FitnessType> fitnessItems;
        private GenderType geschlecht;
        private JobTypes job;
        private FitnessType fitness;
        public object test;

        public event EventHandler<OnSignUpEventArgs> onSignUpComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.dialog_sign_up, container, false);

            txtUsername = view.FindViewById<EditText>(Resource.Id.txtUsername);
            txtEmail = view.FindViewById<EditText>(Resource.Id.txtEmail);
            txtPassword = view.FindViewById<EditText>(Resource.Id.txtPassword);
            txtPasswordConfirm = view.FindViewById<EditText>(Resource.Id.txtPasswordConfirm);
            spinGender = view.FindViewById<Spinner>(Resource.Id.spinGender);
            spinJob = view.FindViewById<Spinner>(Resource.Id.spinJob);
            spinFitness = view.FindViewById<Spinner>(Resource.Id.spinFitness);
            txtBirthdate = view.FindViewById<EditText>(Resource.Id.txtBirthdate);
            btnSignUp = view.FindViewById<Button>(Resource.Id.btnDialogEmail);

            //Gender-Spinner
            genderItems = new List<GenderType>();
            genderItems.Add(GenderType.Male);
            genderItems.Add(GenderType.Female);
            ArrayAdapter<GenderType> genderAdpater = new ArrayAdapter<GenderType>(this.Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, genderItems);
            spinGender.Adapter = genderAdpater;

            //Job-Spinner
            jobItems = new List<JobTypes>();
            jobItems.Add(JobTypes.Easy);
            jobItems.Add(JobTypes.Middle);
            jobItems.Add(JobTypes.Hard);
            ArrayAdapter<JobTypes> jobAdpater = new ArrayAdapter<JobTypes>(this.Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, jobItems);
            spinJob.Adapter = jobAdpater;

            //Fitness-Spinner
            fitnessItems = new List<FitnessType>();
            fitnessItems.Add(FitnessType.NoSport);
            fitnessItems.Add(FitnessType.OnceAWeek);
            fitnessItems.Add(FitnessType.MoreThanOnceAWeek);
            fitnessItems.Add(FitnessType.HighPerformanceAthletes);
            ArrayAdapter<FitnessType> fitnessAdpater = new ArrayAdapter<FitnessType>(this.Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, fitnessItems);
            spinFitness.Adapter = fitnessAdpater;

            btnSignUp.Click += BtnSignUp_Click;

            return view;
        }

        //TODO !!!!
        //GenderType,... zurückgeben aus den Spinnern


        private void BtnSignUp_Click(object sender, EventArgs e)
        {
            if(spinGender.SelectedItem!=null)
            {
                Console.WriteLine(spinGender.SelectedItem.GetType().ToString());
            }
            //User has clicked the SignUp-Button
            onSignUpComplete.Invoke(this, new OnSignUpEventArgs
                (txtUsername.Text, txtEmail.Text, txtPassword.Text, txtPasswordConfirm.Text,GenderType.Male, JobTypes.Easy, FitnessType.HighPerformanceAthletes, new DateTime()));
            //Dialog will slide to the side and will disapear
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //sets the animation
        }
    }

    public class OnSignUpEventArgs : EventArgs
    {
        private string username;
        private string email;
        private string password;
        private string passwordConfirm;
        private GenderType gender;
        private JobTypes job;
        private FitnessType fitness;
        private DateTime birthdate;
        
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string PasswordConfirm
        {
            get { return passwordConfirm; }
            set { passwordConfirm = value; }
        }

        public GenderType Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        public JobTypes Job
        {
            get { return job; }
            set { job = value; }
        }
        
        public FitnessType Fitness
        {
            get { return fitness; }
            set { fitness = value; }
        }

        public DateTime Birthdate
        {
            get { return birthdate; }
            set { birthdate = value; }
        }

        public OnSignUpEventArgs(
            string username, 
            string email, 
            string password, 
            string passwordConfirm, 
            GenderType gender,
            JobTypes job,
            FitnessType fitness,
            DateTime birthdate
            ) : base()
        {
            Username = username;
            Email = email;
            Password = password;
            PasswordConfirm = passwordConfirm;
            Gender = gender;
            Job = job;
            Fitness = fitness;
            Birthdate = birthdate;
        }
    }
}