using System;
using System.Net;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fIT.WebApi.Client.Data.Models.Exercise;
using fIT.WebApi.Client.Data.Models.Schedule;
using fIT.WebApi.Client.Data.Models.Shared.Enums;
using fIT.WebApi.Client.Portable.Implementation;
using fIT.WebApi.Client.Portable.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Nito.AsyncEx.UnitTests;

namespace fIT.WebApi.Client.Portable.Tests
{
    [TestClass]
    public partial class ClientTests
    {
        #region Field

#if DEBUG
        private const string ServiceUrl = "http://localhost:62816/";
#else
        private const string ServiceUrl = "http://fit-bachelor.azurewebsites.net/";
#endif

        private const string USERNAME = "Stefan";
        private const string PASSWORD = "Test1234";

        #endregion

        [TestInitialize]
        public void Setup()
        {
            // ignore invalid ssl certs
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        #region Tests
        [TestMethod]
        public async Task Login()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (var session = await service.LoginAsync(USERNAME, PASSWORD))
            {
                Assert.IsNotNull(session);
            }
        }

        [TestMethod]
        public async Task PerformRefresh()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (var session = await service.LoginAsync(USERNAME, PASSWORD))
            {
                var oldValue = session.Token;
                await session.PerformRefreshAsync();
                var newValue = session.Token;
                Assert.AreNotEqual(oldValue, newValue);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ServerException))]
        public async Task FailLoginInvalidUsername()
        {
            using (var service = new ManagementService(ServiceUrl))
            {
                using (var session = await service.LoginAsync("xyz", PASSWORD))
                {
                }
            }
        }

        [TestMethod]
        public void UserCanRegisterAndDeleteUser()
        {
            var newUser = new CreateUserModel()
            {
                Username = "user" + Environment.TickCount,
                Password = PASSWORD,
                Job = JobTypes.Easy,
                Gender = GenderType.Male,
                Fitness = FitnessType.HighPerformanceAthletes,
                ConfirmPassword = PASSWORD,
                DateOfBirth = DateTime.Today,
                Email = "Test@test.de"
            };
            var registrationSuccessful = false;
            using (var service = new ManagementService(ServiceUrl))
            {
                try
                {
                    service.RegisterAsync(newUser).Wait();
                    registrationSuccessful = true;
                    using (var newUserSession = service.LoginAsync(newUser.Username, newUser.Password).Result)
                    {
                    }
                }
                catch (AggregateException e)
                {

                }
                finally
                {
                    if (registrationSuccessful)
                    {
                        using (var rootsession = service.LoginAsync(USERNAME, PASSWORD).Result)
                        {
                            var user = rootsession.Admins.GetUserByUsernameAsync(newUser.Username).Result;
                            var newUserId = user.Id;
                            rootsession.Admins.DeleteUserAsync(newUserId).Wait();
                        }
                    }
                }
            }
        }


      [TestMethod]
      public void Ping()
      {
        using (var service = new ManagementService(ServiceUrl))
        {
          var peng = false;
          try
          {
            peng = service.PingAsync().Result;
            Assert.IsTrue(peng);

          }
          catch (Exception e)
          {
            throw e;
          }
        }
      }
        #endregion

        #region Helper
        /// <summary>
        /// Erstellt ein Test Trainingsplan
        /// </summary>
        /// <param name="session">aktuelle session</param>
        /// <param name="userId">optionale UserId. Wenn nciht gefüllt, wird die UserId der session benutzt</param>
        /// <returns></returns>
        private Temporary<ScheduleModel> EnsureSchedule(IManagementSession session, string userId = null)
        {
            var newSchedule = new ScheduleModel()
            {
                Name = "Test_Schedule" + System.Environment.TickCount,
                UserId = userId ?? session.CurrentUserId.ToString(),
            };
            var result = session.Users.CreateScheduleAsync(newSchedule).Result;
            return new Temporary<ScheduleModel>(result, x => session.Users.DeleteScheduleAsync(x.Id));
        }

        private Temporary<ExerciseModel> EnsureExercise(IManagementSession session)
        {
            var exercise = new ExerciseModel()
            {
                Name = "Test_Exercise" + System.Environment.TickCount,
                Description = "Awesome Description"
            };
            var result = session.Admins.CreateExerciseAsync(exercise).Result;
            return new Temporary<ExerciseModel>(result, x=> session.Admins.DeleteExerciseAsync(x.Id));
        } 

        #endregion
    }
}
