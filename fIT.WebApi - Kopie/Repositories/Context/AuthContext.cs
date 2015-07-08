using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using fIT.WebApi.Entities;

namespace fIT.WebApi.Repositories.Context
{
    /// <summary>
    /// DB Context to communicate with the DB
    /// </summary>
    public class AuthContext : IdentityDbContext<UserInformation>
    {
        #region Ctor
        public AuthContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates Bindings between objects
        /// </summary>
        /// <param name="modelBuilder"></param>
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

            modelBuilder.Entity<UserInformation>().HasKey(e => e.UserID);
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Creates a new DB Context. Is called from the OWIN Middleware
        /// </summary>
        /// <returns></returns>
        public static AuthContext Create()
        {
            return new AuthContext();
        }
        #endregion

        #region Properties
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Practice> Practices { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        #endregion
    }
}
