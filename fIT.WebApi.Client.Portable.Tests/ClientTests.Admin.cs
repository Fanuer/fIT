using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Portable.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fIT.WebApi.Client.Portable.Tests
{
    public partial class ClientTests
    {
        [TestMethod]
        public void GetUserByName()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                var user = session.Admins.GetUserByUsernameAsync(USERNAME).Result;
                Assert.IsNotNull(user);
                Assert.AreEqual(USERNAME, user.UserName);
            }
        }
    }
}
