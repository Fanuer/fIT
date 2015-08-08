using System;
using Android.App;
using Android.Content;
using Android.OS;
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

        /*
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
        */

        /// <summary>
        /// Pingt den Server an, um zu kontrollieren, ob eine Verbindung dorthin besteht
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsPingReachable()
        {
            return await Task.Run(() =>
            {
                //string host = "216.58.192.35";
                //if (!IsConnected)
                //return false;
                bool reachable;
                try
                {
                    reachable = InetAddress.GetByName("216.58.192.35").IsReachable(1);
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