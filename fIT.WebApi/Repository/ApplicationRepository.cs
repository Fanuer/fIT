using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using fIT.WebApi.Entities;
using fIT.WebApi.Models;
using fIT.WebApi.Repository.Interfaces;
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
        public IExerciseRepository Excercies { get; }
        public IClientRepository Clients { get; private set; }
        #endregion

        #region Sealed Classes

        private class ClientRepository : IClientRepository
        {
            #region Field
            private ApplicationDbContext _ctx;
            #endregion

            #region Ctor

            public ClientRepository(ApplicationDbContext ctx)
            {
                _ctx = ctx;
            }
            #endregion

            #region Methods
            public async Task<Client> FindAsync(string clientId)
            {
                return await _ctx.Clients.FindAsync(clientId);
            }

            #endregion

            #region Properties
            #endregion
        }

        private class RefreshTokenRepository : IRefreshTokenRepository
        {
            #region Field
            private ApplicationDbContext _ctx;
            #endregion

            #region Ctor

            public RefreshTokenRepository(ApplicationDbContext ctx)
            {
                _ctx = ctx;
            }
            #endregion

            #region Methods
            public async Task<bool> AddAsync(RefreshToken token)
            {
                var existingToken = _ctx.RefreshTokens.SingleOrDefault(r => r.Subject == token.Subject && r.ClientId == token.ClientId);

                if (existingToken != null)
                {
                    await RemoveAsync(existingToken);
                }
                _ctx.RefreshTokens.Add(token);
                return await _ctx.SaveChangesAsync() > 0;
            }

            public async Task<bool> RemoveAsync(string refreshTokenId)
            {
                var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

                if (refreshToken != null)
                {
                    _ctx.RefreshTokens.Remove(refreshToken);
                    return await _ctx.SaveChangesAsync() > 0;
                }

                return false;
            }

            public async Task<bool> RemoveAsync(RefreshToken refreshToken)
            {
                _ctx.RefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }

            public async Task<RefreshToken> FindAsync(string refreshTokenId)
            {
                return await _ctx.RefreshTokens.FindAsync(refreshTokenId);
            }

            public IQueryable<RefreshToken> GetAllAsync()
            {
                return _ctx.RefreshTokens;
            }

            #endregion

            #region Properties

            #endregion

        }

        private class ExerciseRepository : IExerciseRepository
        {
            #region Field
            private ApplicationDbContext _ctx;
            #endregion

            #region Ctor
            public ExerciseRepository(ApplicationDbContext ctx)
            {
                _ctx = ctx;
            }

            #endregion

            #region Methods
            public IQueryable<Exercise> GetAllAsync()
            {
                return this._ctx.Exercises;
            }

            public async Task<Exercise> FindAsync(int id)
            {
                return await this._ctx.Exercises.FindAsync(id);
            }

            public async Task UpdateAsync(int id, Exercise model)
            {
                if (model == null)
                {
                    throw new ArgumentNullException("model");
                }
                _ctx.Entry(model).State = EntityState.Modified;
                await this._ctx.SaveChangesAsync();
            }

            public async Task<Exercise> AddAsync(Exercise model)
            {
                if (model == null)
                {
                    throw new ArgumentNullException("model");
                }
                this._ctx.Exercises.Add(model);
                var id = await this._ctx.SaveChangesAsync();
                return await this.FindAsync(id);
            }

            public async Task RemoveAsync(Exercise model)
            {
                if (model == null)
                {
                    throw new ArgumentNullException("model");
                }

                this._ctx.Exercises.Remove(model);
                await this._ctx.SaveChangesAsync();
            }

            public async Task<bool> ExistsAsync(int id)
            {
                return (await this._ctx.Exercises.CountAsync(e => e.Id == id)) > 0;
            }
            #endregion

            #region Properties
            #endregion


        }

        #endregion
    }


}