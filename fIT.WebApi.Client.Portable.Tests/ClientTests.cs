using System;
using System.Net;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fIT.WebApi.Client.Data.Models.Shared.Enums;
using fIT.WebApi.Client.Portable.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx.UnitTests;

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
        public async Task UserCanRegisterAndDeleteUser()
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
                    await service.RegisterAsync(newUser);
                    registrationSuccessful = true;
                    using (var newUserSession = await service.LoginAsync(newUser.Username, newUser.Password))
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
                        using (var rootsession = await service.LoginAsync(USERNAME, PASSWORD))
                        {
                            var user = await rootsession.Admins.GetUserByUsernameAsync(newUser.Username);
                            var newUserId = user.Id;
                            await rootsession.Admins.DeleteUserAsync(newUserId);
                        }
                    }
                }
            }
        }
    }
}
