using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Exercise;
using fIT.WebApi.Client.Data.Models.Practice;
using fIT.WebApi.Client.Data.Models.Schedule;

namespace fIT.WebApi.Client.Data.Intefaces
{
    public interface IUserManagement
    {
        /// <summary>
        /// Ändert das Nutzerpasswort
        /// </summary>
        /// <param name="model">password data</param>
        /// <returns></returns>
        Task UpdatePasswordAsync(ChangePasswordModel model);
        /// <summary>
        /// Ruft die Daten das aktuell angemeldeten Nutzers ab
        /// </summary>
        /// <returns></returns>
        Task<UserModel> GetUserDataAsync();
        /// <summary>
        /// Ändert die Daten des aktuell angemeldeten Nutzers
        /// </summary>
        /// <param name="newData"></param>
        /// <returns></returns>
        Task UpdateUserDataAsync(UserModel newData);

        #region Schedules

        /// <summary>
        /// Gibt einen Trainingsplan anhand der Id wieder
        /// </summary>
        /// <param name="id">Id eines Trainingsplans</param>
        /// <returns></returns>
        Task<ScheduleModel> GetScheduleByIdAsync(int id);
        /// <summary>
        /// Gibt alle Trainingsplaene des angemeldeten Nutzers zurueck
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ScheduleModel>> GetAllSchedulesAsync();
        /// <summary>
        /// Erstellt einen neuen Trainingsplan
        /// </summary>
        /// <param name="model">Daten eines Trainingsplans</param>
        /// <returns></returns>
        Task<ScheduleModel> CreateScheduleAsync(ScheduleModel model);
        /// <summary>
        /// Löscht einen Trainingsplans
        /// </summary>
        /// <param name="id">Id des Trainingsplans</param>
        /// <returns></returns>
        Task DeleteScheduleAsync(int id);
        /// <summary>
        /// Ändert einen Trainingsplan
        /// </summary>
        /// <param name="scheduleId">Id des zu aendernden Trainingsplans</param>
        /// <param name="model">neue Daten des Trainingsplans</param>
        /// <returns></returns>
        Task UpdateScheduleAsync(int scheduleId, ScheduleModel model);
        /// <summary>
        /// Fuegt eine Trainingsplan eine Uebung hinzu
        /// </summary>
        /// <param name="scheduleId">Id eines Trainingsplans</param>
        /// <param name="exerciseId">Id einer Uebung</param>
        /// <returns></returns>
        Task AddExerciseToScheduleAsync(int scheduleId, int exerciseId);
        /// <summary>
        /// Entfernt eine Uebung von einem Trainingsplan
        /// </summary>
        /// <param name="scheduleId">Id eines Trainingsplans</param>
        /// <param name="exerciseId">Id einer Uebung</param>
        /// <returns></returns>
        Task RemoveExerciseFromScheduleAsync(int scheduleId, int exerciseId);

        #endregion

        #region Exercise

        Task<IEnumerable<ExerciseModel>> GetAllExercisesAsync();
        Task<ExerciseModel> GetExerciseByIdAsync(int exerciseId);

        #endregion

        #region Practice    

        /// <summary>
        /// Gibt alle Trainings eines Nutzers zurueck
        /// </summary>
        Task<IEnumerable<PracticeModel>> GetAllPracticesAsync();
        /// <summary>
        /// Gibt eine Trainingseinheit anhand der Id zurueck
        /// </summary>
        /// <param name="id">id eines Trainings</param>
        /// <returns></returns>
        Task<PracticeModel> GetPracticeByIdAsync(int id);
        /// <summary>
        /// Erstellt ein neues Training fuer den angemeldeten Nutzer
        /// </summary>
        /// <param name="model">Trainingsdaten</param>
        /// <returns></returns>
        Task<PracticeModel> CreatePracticeAsync(PracticeModel model);
        /// <summary>
        /// Loescht ein Training
        /// </summary>
        /// <param name="id">id des Datensatzes</param>
        /// <returns></returns>
        Task DeletePracticeAsync(int id);
        /// <summary>
        /// Aendert ein Training ab
        /// </summary>
        /// <param name="id">id des trainings</param>
        /// <param name="model">neue Daten</param>
        /// <returns></returns>
        Task UpdatePracticeAsync(int id, PracticeModel model);

    #endregion

    #region PROPERTIES

      Guid CurrentUserID { get; set; }

      #endregion
  }
}
