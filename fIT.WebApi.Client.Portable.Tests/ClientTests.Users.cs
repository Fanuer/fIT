using System;
using System.Runtime.InteropServices.WindowsRuntime;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Portable.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fIT.WebApi.Client.Portable.Tests
{
    public partial class ClientTests
    {
        [TestMethod]
        public void GetCurrentUserData()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                var data = session.Users.GetUserDataAsync().Result;
                Assert.AreEqual(USERNAME, data.UserName);
            }
        }

        [TestMethod]
        public void UpdateCurrentUserData()
        {
            const string NEWNAME = "McAwesome";

            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                var data = session.Users.GetUserDataAsync().Result;
                Assert.AreEqual(USERNAME, data.UserName);

                try
                {
                    data.UserName = NEWNAME;
                    session.Users.UpdateUserDataAsync(data).Wait();

                    data = session.Users.GetUserDataAsync().Result;
                    Assert.AreEqual(NEWNAME, data.UserName);
                }
                finally
                {
                    data.UserName = USERNAME;
                    session.Users.UpdateUserDataAsync(data).Wait();
                }
            }
        }

        [TestMethod]
        public void ChangePassword()
        {
            const string NEW_PASSWORD = "Pass1234";
            var new_password = new ChangePasswordModel()
            {
                OldPassword = PASSWORD,
                NewPassword = NEW_PASSWORD,
                ConfirmPassword = NEW_PASSWORD
            };

            var old_password = new ChangePasswordModel()
            {
                OldPassword = NEW_PASSWORD,
                NewPassword = PASSWORD,
                ConfirmPassword = PASSWORD
            };

            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                try
                {
                    session.Users.UpdatePasswordAsync(new_password).Wait();
                    using (IManagementSession newsession = service.LoginAsync(USERNAME, NEW_PASSWORD).Result)
                    {
                        var user = newsession.Users.GetUserDataAsync();
                    }
                }
                finally
                {
                    session.Users.UpdatePasswordAsync(old_password).Wait();
                }
            }
        }
    }
}
