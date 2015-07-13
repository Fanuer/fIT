using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Entities;
using fIT.WebApi.Models;

namespace fIT.WebApi.Repository.Interfaces
{
    public interface IExerciseRepository
    {
        #region Field
        #endregion

        #region Ctor
        #endregion

        #region Methods

        IQueryable<Exercise> GetAllAsync();
        Task<Exercise> FindAsync(int id);
        Task UpdateAsync(int id, Exercise model);
        Task<Exercise> AddAsync(Exercise model);
        Task RemoveAsync(Exercise model);
        Task<bool> ExistsAsync(int id);

        #endregion

        #region Properties

        #endregion

    }
}
