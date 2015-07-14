using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Repository.Interfaces.CRUD
{
  internal interface IRepositoryFindAll<T>
  {
    IQueryable<T> GetAllAsync();
  }
}
