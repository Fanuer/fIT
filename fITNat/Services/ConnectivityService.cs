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
using System.Threading;
using System.Threading.Tasks;
using Android.Net;
using Java.Net;

namespace fITNat
{
    [Service]
    class ConnectivityService : Service
    {
        private ConnectivityManager connectivityManager;

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Console.WriteLine("ConnectivityService gestartet!");
            return StartCommandResult.Sticky;
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                    NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
                    return ((activeConnection != null) && activeConnection.IsConnected);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to get connected state - do you have ACCESS_NETWORK_STATE permission? - error: {0}", e.StackTrace);
                    return false;
                }
            }
        }

        public async Task<bool> IsPingReachable()
        {
            string host = @"http://fit-bachelor.azurewebsites.net/";
            if (!IsConnected)
                return false;

            return await Task.Run(() =>
            {
                bool reachable;
                try
                {
                    reachable = InetAddress.GetByName(host).IsReachable(0);
                }
                catch (UnknownHostException)
                {
                    reachable = false;
                }
                return reachable;
            });
        }


        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
}