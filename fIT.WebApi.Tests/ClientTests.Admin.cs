using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Client.Implementation;
using fIT.WebApi.Client.Intefaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fIT.WebApi.Tests
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
