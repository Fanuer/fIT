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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace fIT.WebApi.Repository
{
  public class ApplicationRepository : IRepository
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
      Excercies = new ExerciseRepository(_ctx);
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
    public IExerciseRepository Excercies { get; private set; }
    public IClientRepository Clients { get; private set; }
    public IScheduleRepository Schedules { get; private set; }
    public IPracticeRepository Practices { get; private set; }

    #endregion

    #region Sealed Classes

    private abstract class GenericRepository<T, TIdProperty> : IRepositoryAddAndDelete<T, TIdProperty>, IRepositoryFindAll<T>, IRepositoryFindSingle<T, TIdProperty>, IRepositoryUpdate<T, TIdProperty> where T : IEntity<TIdProperty>
    {
      #region Field

      private ApplicationDbContext _ctx;
      #endregion

      #region Ctor

      public GenericRepository(ApplicationDbContext ctx)
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
        this._ctx.Set(typeof (T)).Add(model);
        return await _ctx.SaveChangesAsync() > 0;
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
        return await this.GetAllAsync().CountAsync(e => e.Id.Equals(id)) > 0;
      }

      public IQueryable<T> GetAllAsync()
      {
        return this._ctx.Set(typeof(T)) as IQueryable<T>;
      }

      public async Task<T> FindAsync(TIdProperty id)
      {
        return (T)await this._ctx.Set(typeof(T)).FindAsync(id);
      }

      public async Task<bool> UpdateAsync(TIdProperty id, T model)
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
      public ClientRepository(ApplicationDbContext ctx): base(ctx){}
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