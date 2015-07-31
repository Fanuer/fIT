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

namespace fITNat
{
    [Activity(Label = "fITNat")]
    public class PracticeActivity : Activity
    {
        private ImageView connectivityPointer;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.PracticeLayout);

            List<PracticeModel> practices = new List<PracticeModel>();
            /*for (int i = 0; i < 10; i++)
            {
                practices.Add(i + ". Practice");
            }*/

            ArrayAdapter<PracticeModel> adapter = new ArrayAdapter<PracticeModel>(this, Resource.Layout.PracticeView, Resource.Id.txtPracticeViewDescription, practices);
            ListView lv = (ListView)FindViewById(Resource.Id.lvPractice);
            connectivityPointer = FindViewById<ImageView>(Resource.Id.ivConnectionPractice);
            lv.Adapter = adapter;
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
            if (online)
                connectivityPointer.SetBackgroundResource(Resource.Drawable.CheckDouble);
            else
                connectivityPointer.SetBackgroundResource(Resource.Drawable.Check);
        }
    }
}