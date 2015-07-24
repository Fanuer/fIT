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
using Android.Util;

namespace fITNat
{
    [Service]
    class OnOffService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        //OnCreate -> Initialisierung

        //public StartCommandResult OnStartCommand -> Handling des Neustarts des Services
        //Sticky -> startet bei null
        //RedeliverIntent -> macht da weiter wo er aufgehört hat
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("DemoService", "DemoService started");
            return StartCommandResult.Sticky;
        }

        //OnDestroy -> Nach Beendigung
        public override void OnDestroy()
        {
            base.OnDestroy();
            // cleanup code
        }

        public bool asyncPing()
        {
            /*
            while(asyncPing == true)
            {
                
            }
            //call http://fit-bachelor.azurewebsites.net:80/api/Accounts/Ping
            */
            return true;
        }
    }
}