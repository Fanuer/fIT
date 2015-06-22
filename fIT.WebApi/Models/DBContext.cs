using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace fIT.WebApi.Models
{
    public class DBContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public DBContext() : base("name=DBContext")
        {
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exercise>()
                .HasMany(exersice => exersice.Schedules)
                .WithMany(schedule => schedule.Exercises)
                .Map(exercises =>
                {
                    exercises.MapLeftKey("ExerciseRefId");
                    exercises.MapRightKey("SchaduleRefId");
                    exercises.ToTable("ScheduleExercise");
                });


            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Practice> Practices { get; set; }

    }
}
