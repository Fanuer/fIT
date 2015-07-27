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
using fIT.WebApi.Client.Portable.Implementation;
using System.Threading.Tasks;

namespace fITNat.Services
{
    [Service]
    class ManagementServiceLocal : Service
    {
        private const string URL = @"http://fit-bachelor.azurewebsites.net/";
        private ManagementService service;
        private const string USERNAME = "Kevin";
        private const string PASSWORD = "test1234";

        public async override void OnCreate()
        {
            //Testweise
            service = new ManagementService(URL);
            var test = service.LoginAsync(USERNAME, PASSWORD);
            //var session = service.LoginAsync(USERNAME, PASSWORD);
            base.OnCreate();
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
}