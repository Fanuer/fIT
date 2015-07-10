using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Entities;

namespace fIT.WebApi.Repository.Interfaces
{
  public interface IClientRepository
  {
    Task<Client> FindAsync(string clientId);
  }
}
