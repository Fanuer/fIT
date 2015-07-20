using System;
using System.Net;
using fIT.WebApi.Client.Implementation;
using fIT.WebApi.Client.Models;
using fIT.WebApi.Client.Models.Account;
using fIT.WebApi.Client.Models.Exceptions;
using fIT.WebApi.Client.Models.Shared.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fIT.WebApi.Tests
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

        [TestMethod]
        public void Login()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (var session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                Assert.IsNotNull(session);
            }
        }

        [TestMethod]
        public void PerformRefresh()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (var session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                var oldValue = session.Token;
                session.PerformRefreshAsync().Wait();
                var newValue = session.Token;
                Assert.AreNotEqual(oldValue, newValue);
            }
        }

        [TestMethod]
        public void FailLoginInvalidUsername()
        {
            using (var service = new ManagementService(ServiceUrl))
            {
                try
                {
                    using (var session = service.LoginAsync("xyz", PASSWORD).Result)
                    {
                    }

                    Assert.Fail();
                }
                catch (AggregateException e)
                {
                    Assert.IsTrue(e.InnerException is ServerException);
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
                finally
                {
                    if (registrationSuccessful)
                    {
                        using (var rootsession = service.LoginAsync(USERNAME, PASSWORD).Result)
                        {
                            var newUserId = rootsession.Admins.GetUserByUsernameAsync(newUser.Username).Result.Id;
                            rootsession.Admins.DeleteUserAsync(newUserId).Wait();
                        }
                    }
                }
            }
        }
    }
}
