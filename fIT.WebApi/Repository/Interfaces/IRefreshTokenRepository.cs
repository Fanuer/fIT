using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using fIT.WebApi.Entities;

namespace fIT.WebApi.Repository.Interfaces
{
  public interface IRefreshTokenRepository
  {
    Task<bool> AddAsync(RefreshToken token);
    Task<bool> RemoveAsync(string refreshTokenId);
    Task<bool> RemoveAsync(RefreshToken refreshToken);
    Task<RefreshToken> FindAsync(string refreshTokenId);
    Task<List<RefreshToken>> GetAllAsync();
  }
}