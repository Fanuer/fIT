using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using fIT.WebApi.Entities;
using fIT.WebApi.Repository.Interfaces.CRUD;

namespace fIT.WebApi.Repository.Interfaces
{
    public interface IRefreshTokenRepository : IRepositoryAddAndDelete<RefreshToken, string>, IRepositoryFindAll<RefreshToken>, IRepositoryFindSingle<RefreshToken, string>
  {
  }
}