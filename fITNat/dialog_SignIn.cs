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
    class dialog_SignIn : DialogFragment
    {
        private EditText txtUsername;
        private EditText txtPassword;
        private Button btnSignIn;

        public event EventHandler<OnSignInEventArgs> onSignInComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.dialog_sign_in, container, false);

            txtUsername = view.FindViewById<EditText>(Resource.Id.txtUsername);
            txtPassword = view.FindViewById<EditText>(Resource.Id.txtPassword);
            btnSignIn = view.FindViewById<Button>(Resource.Id.btnDialogSignIn);


            btnSignIn.Click += BtnSignIn_Click;

            return view;
        }

        private void BtnSignIn_Click(object sender, EventArgs e)
        {
            //User has clicked the Login-Button
            onSignInComplete.Invoke(this, new OnSignInEventArgs
                (txtUsername.Text, txtPassword.Text));
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

    public class OnSignInEventArgs : EventArgs
    {
        private string username;
        private string password;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public OnSignInEventArgs(string username, string password) : base()
        {
            Username = username;
            Password = password;
        }
    }
}