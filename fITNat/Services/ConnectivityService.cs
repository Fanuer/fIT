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
            int msTimeout = 10000;
            if (!IsConnected)
                return false;

            return await Task.Run(() =>
            {
                bool reachable;
                try
                {
                    reachable = InetAddress.GetByName(host).IsReachable(msTimeout);
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