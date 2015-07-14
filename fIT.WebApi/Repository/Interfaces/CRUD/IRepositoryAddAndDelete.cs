using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Repository.Interfaces.CRUD
{
    public interface IRepositoryAddAndDelete<T, in TIdProperty> where T : IEntity<TIdProperty>
  {
    Task<bool> AddAsync(T model);
    Task<bool> RemoveAsync(TIdProperty id);
    Task<bool> RemoveAsync(T model);
    Task<bool> ExistsAsync(TIdProperty id);
  }
}
