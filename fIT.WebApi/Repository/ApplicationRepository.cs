using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using fIT.WebApi.Entities;
using fIT.WebApi.Models;
using fIT.WebApi.Repository.Interfaces;
using fIT.WebApi.Repository.Interfaces.CRUD;
using fIT.WebApi.Repository.Interfaces.CRUD.SingleID;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace fIT.WebApi.Repository
{
    internal class ApplicationRepository : IRepository
    {
        #region Field
        private ApplicationDbContext _ctx;
        private UserManager<IdentityUser> _userManager;
        #endregion

        #region Ctor
        public ApplicationRepository()
        {
            _ctx = new ApplicationDbContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
            RefreshTokens = new RefreshTokenRepository(_ctx);
            Clients = new ClientRepository(_ctx);
            Exercise = new ExerciseRepository(_ctx);
            Schedules = new ScheduleRepository(_ctx);
            Practices = new PracticeRepository(_ctx);
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
        #endregion

        #region Properties

        public IRefreshTokenRepository RefreshTokens { get; private set; }
        public IExerciseRepository Exercise { get; private set; }
        public IClientRepository Clients { get; private set; }
        public IScheduleRepository Schedules { get; private set; }
        public IPracticeRepository Practices { get; private set; }

        #endregion

        #region Sealed Classes

        private abstract class GenericRepository<T, TIdProperty> : IRepositoryAddAndDelete<T, TIdProperty>, IRepositoryFindAll<T>, IRepositoryFindSingle<T, TIdProperty>, IRepositoryUpdate<T, TIdProperty> where T : class, IEntity<TIdProperty>
        {
            #region Field

            protected ApplicationDbContext _ctx;
            #endregion

            #region Ctor

            protected GenericRepository(ApplicationDbContext ctx)
            {
                _ctx = ctx;
            }
            #endregion

            #region Method
            public async Task<bool> AddAsync(T model)
            {
                if (model == null)
                {
                    throw new ArgumentNullException("model");
                }

                var existingModel = await this.FindAsync(model.Id);
                if (existingModel != null)
                {
                    await RemoveAsync(existingModel);
                }
                this._ctx.Set(typeof(T)).Add(model);
                return await _ctx.SaveChangesAsync() > 0;
            }

            public async Task<bool> RemoveAsync(TIdProperty[] id)
            {
                var model = await this.FindAsync(id);
                return await RemoveAsync(model);
            }

            public async Task<bool> RemoveAsync(TIdProperty id)
            {
                var model = await this.FindAsync(id);
                return await RemoveAsync(model);
            }

            public async Task<bool> RemoveAsync(T model)
            {
                if (model == null)
                {
                    throw new ArgumentNullException("model");
                }
                this._ctx.Set(typeof(T)).Remove(model);
                return await this._ctx.SaveChangesAsync() > 0;
            }

            public async Task<bool> ExistsAsync(TIdProperty id)
            {
                return await this.GetAllAsync().AsQueryable<T>().CountAsync(e => e.Id.Equals(id)) > 0;
            }


            public IEnumerable<T> GetAllAsync()
            {
                return this._ctx.Set(typeof(T)) as IEnumerable<T>;
            }

            public async Task<T> FindAsync(TIdProperty id)
            {
                return (T)await this._ctx.Set(typeof(T)).FindAsync(id);
            }

            public async Task<T> FindAsync(TIdProperty[] id)
            {
                return (T)await this._ctx.Set(typeof(T)).FindAsync(id);
            }

            public async Task<bool> UpdateAsync(T model)
            {
                if (model == null)
                {
                    throw new ArgumentNullException("model");
                }
                _ctx.Entry(model).State = EntityState.Modified;
                return await this._ctx.SaveChangesAsync() > 0;
            }


            #endregion

            #region Property

            #endregion
        }

        private class ClientRepository : GenericRepository<Client, string>, IClientRepository
        {
            public ClientRepository(ApplicationDbContext ctx) : base(ctx) { }
        }

        private class RefreshTokenRepository : GenericRepository<RefreshToken, string>, IRefreshTokenRepository
        {
            #region Ctor
            public RefreshTokenRepository(ApplicationDbContext ctx) : base(ctx) { }
            #endregion
        }

        private class ExerciseRepository : GenericRepository<Exercise, int>, IExerciseRepository
        {
            #region Ctor
            public ExerciseRepository(ApplicationDbContext ctx) : base(ctx) { }
            #endregion
        }

        private class ScheduleRepository : GenericRepository<Schedule, int>, IScheduleRepository
        {
            #region Ctor
            public ScheduleRepository(ApplicationDbContext ctx) : base(ctx) { }
            #endregion

            #region Methods
            /// <summary>
            /// Adds an Exercise to a Schedule
            /// </summary>
            /// <param name="scheduleId">Id of a schedule</param>
            /// <param name="exerciseId">Id of an exercise</param>
            /// <returns>true, if adding was successful</returns>
            public async Task<bool> AddExerciseAsync(int scheduleId, int exerciseId)
            {
                var schedule = await _ctx.Schedules.FindAsync(scheduleId);
                if (schedule == null)
                {
                    throw new Exception("Schedule not found for adding an exercise");
                }

                var dbEntry = _ctx.Entry(schedule);
                dbEntry.State = EntityState.Modified;
                await dbEntry.Collection(x=>x.Exercises).LoadAsync();

                var exercise = await _ctx.Exercises.FindAsync(exerciseId);
                if (exercise == null)
                {
                    throw new Exception("Exercise not found for adding to a schedule");
                }
                schedule.Exercises.Add(exercise);
                return await _ctx.SaveChangesAsync() > 0;
            }

            /// <summary>
            /// Removes an Exercise to a Schedule
            /// </summary>
            /// <param name="scheduleId">Id of a schedule</param>
            /// <param name="exerciseId">Id of an exercise</param>
            /// <returns>true, if adding was successful</returns>
             public async Task<bool> RemoveExerciseAsync(int scheduleId, int exerciseId)
            {
                var schedule = await _ctx.Schedules.Include("Exercise").FirstOrDefaultAsync(x => x.Id == scheduleId);
                if (schedule == null)
                {
                    throw new Exception("Schedule not found for adding an exercise");
                }
                var dbEntry = _ctx.Entry(schedule);
                dbEntry.State = EntityState.Modified;
                await dbEntry.Collection(x => x.Exercises).LoadAsync();

                var exercise = await _ctx.Exercises.FindAsync(exerciseId);
                if (exercise == null)
                {
                    throw new Exception("Exercise not found for adding to a schedule");
                }
                if (schedule.Exercises.Any(x=>x.Id == exerciseId))
                {
                    schedule.Exercises.Remove(exercise);
                }
                return await _ctx.SaveChangesAsync() > 0;
            }
            #endregion

        }

        private class PracticeRepository : GenericRepository<Practice, int>, IPracticeRepository
        {
            #region Ctor
            public PracticeRepository(ApplicationDbContext ctx) : base(ctx) { }
            #endregion
        }

        #endregion
    }
}