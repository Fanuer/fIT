using fIT.WebApi.Entities;
using fIT.WebApi.Entities.Enums;
using fIT.WebApi.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace fIT.WebApi.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<fIT.WebApi.Repository.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            var user = new ApplicationUser()
            {
                UserName = "Stefan",
                Email = "Stefan@fIT.com",
                EmailConfirmed = true,
                Age = 27,
                Level = 1,
                Fitness = FitnessType.OnceAWeek,
                Job = JobTypes.Easy,
                Gender = GenderType.Male
            };
            manager.Create(user, "Test1234");

            user = new ApplicationUser()
            {
                UserName = "Kevin",
                Email = "Kevin@fIT.com",
                EmailConfirmed = true,
                Age = 22,
                Level = 1,
                Fitness = FitnessType.HighPerformanceAthletes,
                Job = JobTypes.Easy,
                Gender = GenderType.Male
            };
            manager.Create(user, "Test1234");
        }
    }
}
