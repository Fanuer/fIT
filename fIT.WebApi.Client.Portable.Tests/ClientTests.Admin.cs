using System;
using System.Linq;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Roles;
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

        [TestMethod]
        public void GetUserById()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                var guid = new Guid("7b815457-a918-438d-9697-c1c2b4905648");
                var user = session.Admins.GetUserByIdAsync(guid).Result;
                Assert.IsNotNull(user);
                Assert.AreEqual(USERNAME, user.UserName);
            }
        }

        [TestMethod]
        public void GetAllUsers()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                var users = session.Admins.GetAllUsersAsync().Result;
                Assert.IsNotNull(users);
                Assert.AreEqual(2, users.Count());
                Assert.AreEqual(USERNAME, users.First().UserName);
            }
        }

        [TestMethod]
        public void GetAllRoles()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                var roles = session.Admins.GetAllRolesAsync().Result;
                Assert.AreEqual(2, roles.Count());
                Assert.IsTrue(roles.Select(x => x.Name).Contains("user", StringComparer.CurrentCultureIgnoreCase));
                Assert.IsTrue(roles.Select(x => x.Name).Contains("admin", StringComparer.CurrentCultureIgnoreCase));
            }
        }

        [TestMethod]
        public void GetRoleByName()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                var role = session.Admins.GetRoleByNameAsync("Admin").Result;
                Assert.IsNotNull(role);
                Assert.AreEqual("Admin", role.Name);
                Console.WriteLine("RoleId: {0}", role.Id);
            }
        }

        [TestMethod]
        public void CreateDeleteRole()
        {
            const string ROLENAME = "Testrole";
            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                RoleModel role = null;

                try
                {
                    role = session.Admins.CreateRoleAsync(ROLENAME).Result;
                    role = session.Admins.GetRoleByIdAsync(role.Id).Result;
                    Assert.IsNotNull(role);
                    Assert.AreEqual(ROLENAME, role.Name);
                    Console.WriteLine("RoleId: {0}", role.Id);

                }
                finally
                {
                    if (role != null)
                    {
                        session.Admins.DeleteRoleAsync(role.Id).Wait();
                    }
                }
            }
        }

        [TestMethod]
        public void GetRefreshTokens()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                var tokens = session.Admins.GetAllRefreshtokensAsync().Result;
                Assert.AreNotEqual(0, tokens.Count());
                Console.WriteLine(tokens.Count());
            }
        }

        [TestMethod]
        public void DeleteRefreshToken()
        {
            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                var tokens = session.Admins.GetAllRefreshtokensAsync().Result;
                Assert.AreEqual(1, tokens.Count());
                session.Admins.DeleteRefreshtokenAsync(tokens.First().Id).Wait();
                tokens = session.Admins.GetAllRefreshtokensAsync().Result;
                Assert.AreEqual(0, tokens.Count());
            }
        }
    }
}
