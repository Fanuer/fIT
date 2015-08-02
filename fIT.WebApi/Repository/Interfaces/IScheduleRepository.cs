﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Entities;
using fIT.WebApi.Repository.Interfaces.CRUD;

namespace fIT.WebApi.Repository.Interfaces
{
    public interface IScheduleRepository : IRepositoryAddAndDelete<Schedule, int>, IRepositoryFindAll<Schedule>, IRepositoryFindSingle<Schedule, int>, IRepositoryUpdate<Schedule, int>
    {
        Task<bool> AddExerciseAsync(int scheduleId, int exerciseId);
        Task<bool> RemoveExerciseAsync(int scheduleId, int exerciseId);
    }
}
