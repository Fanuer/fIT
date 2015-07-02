using fIT.WebApi.Models;
using fIT.WebApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace fIT.WebApi.Migrations
{
    public class DBIntitializer: CreateDatabaseIfNotExists<DBContext>
    {
        protected override void Seed(DBContext context)
        {
            context.Users.AddOrUpdate(x => x.ID,
                new ApplicationUser(1, "Stefan", "test", "test@test.de", 27, fitness: FitnessType.OnceAWeek),
                new ApplicationUser(2, "Kevin", "test", "test@test.de", 21, fitness: FitnessType.HighPerformanceAthletes));

            context.Schedules.AddOrUpdate(x => x.ID,
                new Schedule(1, "BOP", 1),
                new Schedule(1, "Hardcore", 2)
                );

            context.Exercises.AddOrUpdate(x => x.ID,
                new Exercise(1, "SitUp", "Das normale Hoch-Runter-Business (liegend)"),
                new Exercise(2, "Klimmzug", "Das normale Hoch-Runter-Business (hängend)"),
                new Exercise(2, "Kniebeuge", "Das normale Hoch-Runter-Business (stehend)")
                );

            context.SaveChanges();
        }
    }
}