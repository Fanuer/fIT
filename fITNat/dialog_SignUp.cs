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

namespace fITNat
{
    class dialog_SignUp : DialogFragment
    {
        private EditText txtUsername;
        private EditText txtEmail;
        private EditText txtPassword;
        private EditText txtBirthdate;
        private Button btnSignUp;

        public event EventHandler<OnSignUpEventArgs> onSignUpComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.dialog_sign_up, container, false);

            txtUsername = view.FindViewById<EditText>(Resource.Id.txtUsername);
            txtEmail = view.FindViewById<EditText>(Resource.Id.txtEmail);
            txtPassword = view.FindViewById<EditText>(Resource.Id.txtPassword);
            txtBirthdate = view.FindViewById<EditText>(Resource.Id.txtBirthdate);
            btnSignUp = view.FindViewById<Button>(Resource.Id.btnDialogEmail);

            btnSignUp.Click += BtnSignUp_Click;

            return view;
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
        {
            //User has klicked the SignUp-Button
            onSignUpComplete.Invoke(this, new OnSignUpEventArgs
                (txtUsername.Text, txtEmail.Text, txtPassword.Text, txtBirthdate.Text));
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
        private string birthdate;
        
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

        public string Birthdate
        {
            get { return birthdate; }
            set { birthdate = value; }
        }

        public OnSignUpEventArgs(string username, string email, string password, string birthdate) : base()
        {
            Username = username;
            Email = email;
            Password = password;
            Birthdate = birthdate;
        }
    }
}