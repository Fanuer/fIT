using fIT.WebApi.Client.Data.Intefaces;
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
    }
}
