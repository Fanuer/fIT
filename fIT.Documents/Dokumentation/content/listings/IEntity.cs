using System;
using System.Collections.Generic;

namespace fIT.WebApi.Repository.Interfaces.CRUD
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}