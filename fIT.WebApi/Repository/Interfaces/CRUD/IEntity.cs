using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fIT.WebApi.Repository.Interfaces.CRUD
{
  public class IEntity<T>
  {
    public T Id { get; set; }
  }
}