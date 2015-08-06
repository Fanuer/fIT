using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fIT.WebApi.Client.Data.Models.Exercise;
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
                var user = session.Admins.GetUserByIdAsync(session.CurrentUserId).Result;
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
        public void CreateGetUpdateDeleteExercises()
        {
            var newExercise = new ExerciseModel()
            {
                
                Name = "Test Name",
                Description = "Test Description"
            };

            using (var service = new ManagementService(ServiceUrl))
            using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
            {
                ExerciseModel myNewExercise = null;

                try
                {
                    var exercises = session.Users.GetAllExercisesAsync().Result;

                    myNewExercise = session.Admins.CreateExerciseAsync(newExercise).Result;
                    Assert.IsNotNull(myNewExercise);
                    Assert.AreEqual(newExercise.Name, myNewExercise.Name);
                    Assert.AreNotEqual(newExercise.Id, myNewExercise.Id);
                    Assert.AreEqual(newExercise.Description, myNewExercise.Description);
                    var newExercises = session.Users.GetAllExercisesAsync().Result;
                    Assert.AreEqual(exercises.Count() + 1, newExercises.Count());

                    var exercise = session.Users.GetExerciseByIdAsync(myNewExercise.Id).Result;
                    Assert.AreEqual(newExercise.Name, exercise.Name);
                    Assert.AreEqual(newExercise.Description, exercise.Description);
                    Assert.AreNotEqual(newExercise.Id, exercise.Id);

                    exercise.Name = newExercise.Name + " Neu";
                    session.Admins.UpdateExerciseAsync(exercise.Id, exercise).Wait();
                    exercise = session.Users.GetExerciseByIdAsync(exercise.Id).Result;
                    Assert.AreEqual(newExercise.Name + " Neu", exercise.Name);

                    session.Admins.DeleteExerciseAsync(exercise.Id).Wait();
                    newExercises = session.Users.GetAllExercisesAsync().Result;
                    Assert.AreEqual(exercises.Count(), newExercises.Count());
                    myNewExercise = null;
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
                    if (myNewExercise != null)
                    {
                        session.Users.DeleteScheduleAsync(myNewExercise.Id).Wait();
                    }
                }
            }
        }


        /*[TestMethod]
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
        }*/
    }
}
