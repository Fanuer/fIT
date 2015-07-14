﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Entities;
using fIT.WebApi.Models;
using fIT.WebApi.Repository.Interfaces.CRUD;

namespace fIT.WebApi.Repository.Interfaces
{
  public interface IExerciseRepository : IRepositoryAddAndDelete<Exercise, int>, IRepositoryFindAll<Exercise>, IRepositoryFindSingle<Exercise, int>, IRepositoryUpdate<Exercise, int>
  {
  }
}
