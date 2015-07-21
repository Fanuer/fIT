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
    class Dialog_SignUp : DialogFragment
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

        public event EventHandler<OnSignUpEventArgs> onSignUpComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.Dialog_sign_up, container, false);

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
            genderItems = Enum.GetValues(typeof(GenderType)).Cast<GenderType>().ToList();
            ArrayAdapter<GenderType> genderAdpater = new ArrayAdapter<GenderType>(this.Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, genderItems);
            spinGender.Adapter = genderAdpater;

            //Job-Spinner
            jobItems = Enum.GetValues(typeof(JobTypes)).Cast<JobTypes>().ToList();
            ArrayAdapter<JobTypes> jobAdpater = new ArrayAdapter<JobTypes>(this.Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, jobItems);
            spinJob.Adapter = jobAdpater;

            //Fitness-Spinner
            fitnessItems = Enum.GetValues(typeof(FitnessType)).Cast<FitnessType>().ToList();
            ArrayAdapter<FitnessType> fitnessAdpater = new ArrayAdapter<FitnessType>(this.Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, fitnessItems);
            spinFitness.Adapter = fitnessAdpater;

            btnSignUp.Click += BtnSignUp_Click;

            return view;
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
        {
            //User has clicked the SignUp-Button
            onSignUpComplete.Invoke(this, new OnSignUpEventArgs
                (
                    txtUsername.Text, 
                    txtEmail.Text, 
                    txtPassword.Text, 
                    txtPasswordConfirm.Text, 
                    getSelectedGender(spinGender.SelectedItem.ToString()), 
                    getSelectedJob(spinJob.SelectedItem.ToString()), 
                    getSelectedFitness(spinFitness.SelectedItem.ToString()), 
                    new DateTime()
                ));
            //Dialog will slide to the side and will disapear
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //sets the animation
        }

        /// <summary>
        /// Generates GenderType out of string
        /// </summary>
        /// <param name="value">string genderValue</param>
        /// <returns>GenderType of selected value</returns>
        private GenderType getSelectedGender(string value)
        {
            GenderType gTgender;
            GenderType gTout = GenderType.Male; //Geht das auch schöner?
            try
            {
                if (Enum.TryParse<GenderType>(value, true, out gTgender))
                {
                    gTout = (GenderType)Enum.Parse(typeof(GenderType), value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Falscher GenderType wurde übermittelt");
            }
            return gTout;
        }

        /// <summary>
        /// Generates JobType out of string
        /// </summary>
        /// <param name="value">string jobValue</param>
        /// <returns>JobType of selected value</returns>
        private JobTypes getSelectedJob(string value)
        {
            JobTypes jTjob;
            JobTypes jTout = JobTypes.Easy; //Geht das auch schöner?
            try
            {
                if (Enum.TryParse<JobTypes>(value, true, out jTjob))
                {
                    jTout = (JobTypes)Enum.Parse(typeof(JobTypes), value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Falscher JobType wurde übermittelt");
            }
            return jTout;
        }

        /// <summary>
        /// Generates FitnessType out of string
        /// </summary>
        /// <param name="value">string fitnessValue</param>
        /// <returns>FitnessType of selected value</returns>
        private FitnessType getSelectedFitness(string value)
        {
            FitnessType fTfitness;
            FitnessType fTout = FitnessType.NoSport; //Geht das auch schöner?
            try
            {
                if (Enum.TryParse<FitnessType>(value, true, out fTfitness))
                {
                    fTout = (FitnessType)Enum.Parse(typeof(FitnessType), value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Falscher FitnessType wurde übermittelt");
            }
            return fTout;
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