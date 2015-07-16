using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fIT.WebApi.Repository.Interfaces.CRUD
{
  public interface IEntity<T>
  {
    T Id { get; set; }
  }
}