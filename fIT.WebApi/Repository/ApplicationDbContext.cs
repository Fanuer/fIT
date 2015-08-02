using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Entities;
using fIT.WebApi.Models;
using fIT.WebApi.Repository.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;

namespace fIT.WebApi.Repository
{
    /// <summary>
    /// Communcates with the Database
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Ctor
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }
        #endregion

        #region Methods
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        internal static IRepository CreateRepository()
        {
          return new ApplicationRepository();
        }


        /*
        
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


        */
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
