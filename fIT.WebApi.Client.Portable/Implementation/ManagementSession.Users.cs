using System.Collections.Generic;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Exercise;
using fIT.WebApi.Client.Data.Models.Practice;
using fIT.WebApi.Client.Data.Models.Schedule;

namespace fIT.WebApi.Client.Portable.Implementation
{
    public partial class ManagementSession : IUserManagement
    {
        #region Field
        #endregion

        #region Ctor
        #endregion

        #region Methods
        #region User
        /// <summary>
        /// Change the users Password
        /// </summary>
        /// <param name="model">password data</param>
        /// <returns></returns>
        public async Task UpdatePasswordAsync(ChangePasswordModel model)
        {
            await PutAsJsonAsync(model, "/api/accounts/ChangePassword");
        }

        /// <summary>
        /// Get the current users data
        /// </summary>
        /// <returns></returns>
        public async Task<UserModel> GetUserDataAsync()
        {
            return await GetAsync<UserModel>("api/Accounts/CurrentUser");
        }

        /// <summary>
        /// Update the current Users data
        /// </summary>
        /// <param name="newData">new Data</param>
        /// <returns></returns>
        public async Task UpdateUserDataAsync(UserModel newData)
        {
            await PutAsJsonAsync(newData, "api/Accounts/CurrentUser");
        }

        #endregion

        #region Schedule
        /// <summary>
        /// Creates a new Schedule
        /// </summary>
        /// <param name="model">schedule data</param>
        /// <returns></returns>
        public async Task<ScheduleModel> CreateScheduleAsync(ScheduleModel model)
        {
            return await this.PostAsJsonReturnAsync<ScheduleModel, ScheduleModel>(model, "api/schedule");
        }

        /// <summary>
        /// Get all of the users Schedules
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ScheduleModel>> GetAllSchedulesAsync()
        {
            return await this.GetAsync<IEnumerable<ScheduleModel>>("api/schedule");
        }

        /// <summary>
        /// Gets a Schedule by its id
        /// </summary>
        /// <param name="id">schedule id</param>
        /// <returns></returns>
        public async Task<ScheduleModel> GetScheduleByIdAsync(int id)
        {
            return await this.GetAsync<ScheduleModel>("api/schedule/" + id);
        }

        /// <summary>
        /// Deletes a schedule 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteScheduleAsync(int id)
        {
            await this.DeleteAsync("api/schedule/" + id);
        }

        /// <summary>
        /// Updates a schedule
        /// </summary>
        /// <param name="scheduleId">id of the schedule</param>
        /// <param name="model">new module data</param>
        /// <returns></returns>
        public async Task UpdateScheduleAsync(int scheduleId, ScheduleModel model)
        {
            await PutAsJsonAsync(model, "api/schedule/" + scheduleId);
        }
        #endregion

        #region Exercise
        /// <summary>
        /// Get all Exercises
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ExerciseModel>> GetAllExercisesAsync()
        {
            return await this.GetAsync<IEnumerable<ExerciseModel>>("api/exercise");
        }

        /// <summary>
        /// Gets one Exercise by its id
        /// </summary>
        /// <param name="exerciseId">Id of an exercise</param>
        /// <returns></returns>
        public async Task<ExerciseModel> GetExerciseByIdAsync(int exerciseId)
        {
            return await this.GetAsync<ExerciseModel>("api/exercise/" + exerciseId);
        }

        #endregion

        #region Practice
        /// <summary>
        /// Gets all of the users practices
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PracticeModel>> GetAllPracticesAsync()
        {
            return await this.GetAsync<IEnumerable<PracticeModel>>("api/practice");
        }

        /// <summary>
        /// Gets one practice by its id
        /// </summary>
        /// <param name="id">practice id</param>
        /// <returns></returns>
        public async Task<PracticeModel> GetPracticeByIdAsync(int id)
        {
            return await this.GetAsync<PracticeModel>("api/practice/" + id);
        }

        /// <summary>
        /// Creates an new Practice
        /// </summary>
        /// <param name="model">practice data</param>
        /// <returns></returns>
        public async Task<PracticeModel> CreatePracticeAsync(PracticeModel model)
        {
            return await this.PostAsJsonReturnAsync<PracticeModel, PracticeModel>(model, "api/practice");
        }

        /// <summary>
        /// Deletes one practice entry
        /// </summary>
        /// <param name="id">practice id</param>
        /// <returns></returns>
        public async Task DeletePracticeAsync(int id)
        {
            await this.DeleteAsync("api/practice/" + id);
        }

        /// <summary>
        /// Updates one practice entry
        /// </summary>
        /// <param name="id">practice id</param>
        /// <param name="model">practice data</param>
        /// <returns></returns>
        public async Task UpdatePracticeAsync(int id, PracticeModel model)
        {
            await PutAsJsonAsync(model, "api/practice/" + id);
        }

        #endregion
        #endregion

        #region Properties
        public IUserManagement Users { get { return this; } }

        #endregion
    }
}
