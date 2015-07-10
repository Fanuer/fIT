using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using fIT.WebApi.Entities;
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
    public IClientRepository Clients { get; private set; }
    #endregion

    #region Sealed Classes

    private class ClientRepository:IClientRepository
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

      public async Task<List<RefreshToken>> GetAllAsync()
      {
        return await _ctx.RefreshTokens.ToListAsync();
      }

      #endregion

      #region Properties

      #endregion

    }

    #endregion
  }
}