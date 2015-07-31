using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Schedule;
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

        [TestMethod]
        public void CreateGetDeleteSchedule()
        {
            var newSchedule = new ScheduleModel()
            {
                Name = "Test Training"
            };

            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                ScheduleModel myNewSchedule = null;

                try
                {
                    var schedules = session.Users.GetAllSchedulesAsync().Result;
                    Assert.AreEqual(0, schedules.Count());

                    myNewSchedule = session.Users.CreateScheduleAsync(newSchedule).Result;
                    Assert.IsNotNull(myNewSchedule);
                    Assert.AreEqual(newSchedule.Name, myNewSchedule.Name);
                    Assert.AreNotEqual(newSchedule.Id, myNewSchedule.Id);
                    Assert.AreNotEqual(newSchedule.UserId, myNewSchedule.UserId);

                    var schedule = session.Users.GetScheduleByIdAsync(myNewSchedule.Id).Result;
                    Assert.AreEqual(newSchedule.Name, schedule.Name);
                    Assert.AreEqual(newSchedule.Id, schedule.Id);
                    Assert.AreEqual(newSchedule.UserId, schedule.UserId);

                    session.Users.DeleteScheduleAsync(schedule.Id).Wait();
                    schedules = session.Users.GetAllSchedulesAsync().Result;
                    Assert.AreEqual(0, schedules.Count());
                    schedule = null;

                }
                finally
                {
                    if (myNewSchedule != null)
                    {
                        session.Users.DeleteScheduleAsync(myNewSchedule.Id).Wait();
                    }
                }
            }
        }
    }
}
