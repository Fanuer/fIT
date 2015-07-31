using System.Collections.Generic;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Models.Account;
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
        /// Gibt alle Trainingspläe des angemeldeten Nutzers zurück
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

        #endregion
    }
}
