using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fIT.WebApi.Client.Data.Models.Practice;
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
        public void CreateGetUpdateDeleteSchedule()
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
                    newSchedule.UserId = session.CurrentUserId.ToString();
                    var schedules = session.Users.GetAllSchedulesAsync().Result;

                    myNewSchedule = session.Users.CreateScheduleAsync(newSchedule).Result;
                    Assert.IsNotNull(myNewSchedule);
                    Assert.AreEqual(newSchedule.Name, myNewSchedule.Name);
                    Assert.AreNotEqual(newSchedule.Id, myNewSchedule.Id);
                    Assert.AreEqual(newSchedule.UserId, myNewSchedule.UserId);
                    var newSchedules = session.Users.GetAllSchedulesAsync().Result;
                    Assert.AreEqual(schedules.Count() + 1, newSchedules.Count());

                    var schedule = session.Users.GetScheduleByIdAsync(myNewSchedule.Id).Result;
                    Assert.AreEqual(newSchedule.Name, schedule.Name);
                    Assert.AreEqual(newSchedule.UserId, schedule.UserId);
                    Assert.AreNotEqual(newSchedule.Id, schedule.Id);
                    Assert.AreEqual(newSchedule.UserId, schedule.UserId);

                    schedule.Name = newSchedule.Name + " Neu";
                    session.Users.UpdateScheduleAsync(schedule.Id, schedule).Wait();
                    schedule = session.Users.GetScheduleByIdAsync(schedule.Id).Result;
                    Assert.AreEqual(newSchedule.Name + " Neu", schedule.Name);

                    session.Users.DeleteScheduleAsync(schedule.Id).Wait();
                    newSchedules = session.Users.GetAllSchedulesAsync().Result;
                    Assert.AreEqual(schedules.Count(), newSchedules.Count());
                    myNewSchedule = null;
                }
                catch (AggregateException e)
                {
                    if (e.InnerException is ServerException)
                    {
                        var innerExc = e.InnerException as ServerException;
                        foreach (KeyValuePair<string, string> data in innerExc.Data)
                        {
                            Console.WriteLine(String.Format("Fehler '{0}': {1}", data.Key, data.Value));
                        }
                    }
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

        [TestMethod]
        public void CreateGetUpdateDeletePractice()
        {
            var testPractice = new PracticeModel()
            {
                Timestamp = DateTime.UtcNow,
                NumberOfRepetitions = 3,
                Repetitions = 10,
                Weight = 80
            };
            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            using (var schedule = EnsureSchedule(session))
            using (var exercise = EnsureExercise(session))
            {
                try
                {
                    var allPracticesCurrent = session.Users.GetAllPracticesAsync().Result;

                    testPractice.ExerciseId = exercise.Model.Id;
                    testPractice.ScheduleId = schedule.Model.Id;
                    testPractice.UserId = session.CurrentUserId.ToString();

                    var newPractice = session.Users.CreatePracticeAsync(testPractice).Result;

                    Assert.IsNotNull(newPractice);
                    Assert.AreEqual(testPractice.ExerciseId, newPractice.ExerciseId);
                    Assert.AreEqual(testPractice.ScheduleId, newPractice.ScheduleId);
                    Assert.AreEqual(testPractice.NumberOfRepetitions, newPractice.NumberOfRepetitions);
                    Assert.AreEqual(testPractice.Repetitions, newPractice.Repetitions);
                    Assert.AreEqual(testPractice.Weight, newPractice.Weight);
                    Assert.AreNotEqual(testPractice.Id, newPractice.Id);

                    var allPracticesNew = session.Users.GetAllPracticesAsync().Result;
                    Assert.AreEqual(allPracticesCurrent.Count() + 1, allPracticesNew.Count());

                    newPractice = session.Users.GetPracticeByIdAsync(newPractice.Id).Result;
                    Assert.AreEqual(testPractice.ExerciseId, newPractice.ExerciseId);
                    Assert.AreEqual(testPractice.ScheduleId, newPractice.ScheduleId);
                    Assert.AreEqual(testPractice.NumberOfRepetitions, newPractice.NumberOfRepetitions);
                    Assert.AreEqual(testPractice.Repetitions, newPractice.Repetitions);
                    Assert.AreEqual(testPractice.Weight, newPractice.Weight);
                    Assert.AreNotEqual(testPractice.Id, newPractice.Id);

                    newPractice.Weight = 70;

                    session.Users.UpdatePracticeAsync(newPractice.Id, newPractice).Wait();
                    newPractice = session.Users.GetPracticeByIdAsync(newPractice.Id).Result;
                    Assert.AreEqual(70, newPractice.Weight);

                    session.Users.DeletePracticeAsync(newPractice.Id).Wait();
                    allPracticesNew = session.Users.GetAllPracticesAsync().Result;
                    Assert.AreEqual(allPracticesCurrent.Count(), allPracticesNew.Count());
                    testPractice = null;
                }
                catch (AggregateException e)
                {
                    if (e.InnerException is ServerException)
                    {
                        var innerExc = e.InnerException as ServerException;
                        var dataItems = innerExc.Data.Cast<object>();
                        if (dataItems.Any())
                        {
                            var first = dataItems.First();
                            if (first is KeyValuePair<string, string>)
                            {
                                foreach (KeyValuePair<string, string> data in innerExc.Data)
                                {
                                    Console.WriteLine(String.Format("Fehler '{0}': {1}", data.Key, data.Value));
                                }
                            }
                            else if (first is DictionaryEntry)
                            {
                                foreach (DictionaryEntry data in dataItems)
                                {
                                    Console.WriteLine(String.Format("Fehler '{0}': {1}", data.Key, data.Value));
                                }
                            }
                            else
                            {
                                foreach (string data in dataItems)
                                {
                                    Console.WriteLine(data.ToString());
                                }
                            }
                        }

                    }
                }
                catch (ServerException e)
                {
                    var dataItems = e.Data.Cast<object>();
                    if (dataItems.Any())
                    {
                        var first = dataItems.First();
                        if (first is KeyValuePair<string, string>)
                        {
                            foreach (KeyValuePair<string, string> data in dataItems)
                            {
                                Console.WriteLine(String.Format("Fehler '{0}': {1}", data.Key, data.Value));
                            }
                        }
                        else if (first is DictionaryEntry)
                        {
                            foreach (DictionaryEntry data in dataItems)
                            {
                                Console.WriteLine(String.Format("Fehler '{0}': {1}", data.Key, data.Value));
                            }
                        }
                        else
                        {
                            foreach (string data in dataItems)
                            {
                                Console.WriteLine(data.ToString());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (testPractice != null)
                    {
                        session.Users.DeletePracticeAsync(testPractice.Id).Wait();
                    }
                }
            }
        }

    }
}
