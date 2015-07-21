namespace fIT.WebApi.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using fIT.WebApi.Models;
    using fIT.WebApi.Entities.Enums;
    using fIT.WebApi.Repositories.Context;
    using fIT.WebApi.Entities;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;

    internal sealed class DBConfiguration : DbMigrationsConfiguration<AuthContext>
    {
        private string[] _userEmails = { "Stefan@fIT.de", "Kevin@fIT.de" };

        public DBConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AuthContext context)
        {
            context.Users.AddOrUpdate(x => x.Email,
                new UserInformation("Stefan", _userEmails[0], 27, fitness: FitnessType.OnceAWeek),
                new UserInformation("Kevin", _userEmails[1], 21, fitness: FitnessType.HighPerformanceAthletes));

            /*context.Schedules.AddOrUpdate(x => x.ID,
                new Schedule(1, "BOP", 1),
                new Schedule(1, "Hardcore", 2)
                );

            context.Exercises.AddOrUpdate(x => x.ID,
                new Exercise(1, "SitUp", "Das normale Hoch-Runter-Business (liegend)"),
                new Exercise(2, "Klimmzug", "Das normale Hoch-Runter-Business (hängend)"),
                new Exercise(2, "Kniebeuge", "Das normale Hoch-Runter-Business (stehend)")
                );

            this.SetSeedUserPasswords(context);
            */
            context.SaveChanges();
        }

        private void SetSeedUserPasswords(AuthContext context)
        {
            //The UserStore is ASP Identity's data layer. Wrap context with the UserStore.
            UserStore<UserInformation> userStore = new UserStore<UserInformation>(context);

            //The UserManager is ASP Identity's implementation layer: contains the methods.
            //The constructor takes the UserStore: how the methods will interact with the database.
            UserManager<UserInformation> userManager = new UserManager<UserInformation>(userStore);

            //Get the UserId only if the SecurityStamp is not set yet.
            var users = context
                                .Users
                                .Where(x => _userEmails.Contains(x.Email) && string.IsNullOrEmpty(x.SecurityStamp))
                                .Select(x => x.Id);

            foreach (var user in users)
            {
                //If the userId is not null, then the SecurityStamp needs updating.
                if (!string.IsNullOrEmpty(user)) userManager.UpdateSecurityStamp(user);
            }

        }
    }
}
