using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using SQLite;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Practice;
using fIT.WebApi.Client.Data.Models.Schedule;
using fIT.WebApi.Client.Data.Models.Exercise;
using fIT.fITNat;

namespace fITNat
{
    class LocalDB
    {
        private static readonly string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private static readonly string path = System.IO.Path.Combine(folder, "app.db");

        public async Task<bool> createDatabase()
        {
            try
            {
                var connection = new SQLiteAsyncConnection(path);
                await connection.CreateTableAsync<UserLoginModel>();
                Console.WriteLine("User geht");
                await connection.CreateTableAsync<ScheduleModel>();
                Console.WriteLine("Schedule geht");
                //await connection.CreateTableAsync<ExerciseModel>();
                //Console.WriteLine("Exercise geht");
                //await connection.CreateTableAsync<PracticeModel>();
                //Console.WriteLine("Practice geht");
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Fehler beim Anlegen der DB(SQLite-Fehler): " + ex.StackTrace);
                return false;
            }
            catch(Exception exc)
            {
                Console.WriteLine("Fehler beim Anlegen der DB: " + exc.StackTrace + "" + exc.GetType());
                return false;
            }
        }

        #region UserLogin
        /// <summary>
        /// Speichert oder updatet einen User in der lokalen DB
        /// </summary>
        /// <param name="data">User</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>0 -> Update</returns>
        /// <returns>1 -> Insert</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> insertUpdateUser(UserLoginModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                if (await db.InsertAsync(data) != 0)
                {
                    await db.UpdateAsync(data);
                    return 0;
                }
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Löscht einen User aus der lokalen DB
        /// </summary>
        /// <param name="data">User</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>1 -> Delete</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> deleteUser(UserLoginModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.DeleteAsync(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Sucht einen User in der lokalen DB
        /// </summary>
        /// <param name="data">User</param>
        /// <param name="path"></param>
        /// <returns>1 -> Vorhanden</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findUser(UserLoginModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.FindAsync<UserModel>(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Sucht einen Benutzer an Hand der Login-Daten in der lokalen DB
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<int> findUser(string username, string password)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                //await db.QueryAsync<String>("Select username from User where username=:username and password=:password", );
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Gibt die Anzahl der gespeicherten User der lokalen DB zurück
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Anzahl der User</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findNumberOfUsers()
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                var count = await db.ExecuteScalarAsync<int>("SELECT Count(*) FROM User");

                // for a non-parameterless query
                // var count = db.ExecuteScalarAsync<int>("SELECT Count(*) FROM User WHERE Username="Amy");

                return count;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        #endregion

        #region Practice
        /// <summary>
        /// Speichert oder updatet ein Training in der lokalen DB
        /// </summary>
        /// <param name="data">Prcatice</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>0 -> Update</returns>
        /// <returns>1 -> Insert</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> insertUpdatePractice(PracticeModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                if (await db.InsertAsync(data) != 0)
                {
                    await db.UpdateAsync(data);
                    return 0;
                }
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Löscht ein Training aus der lokalen DB
        /// </summary>
        /// <param name="data">Practice</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>1 -> Delete</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> deletePractice(PracticeModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.DeleteAsync(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Sucht ein Training in der lokalen DB
        /// </summary>
        /// <param name="data">Practice</param>
        /// <param name="path"></param>
        /// <returns>1 -> Vorhanden</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findPractice(PracticeModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.FindAsync<PracticeModel>(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Gibt die Anzahl der gespeicherten Trainings der lokalen DB zurück
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Anzahl der Trainings</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findNumberOfPractices()
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                var count = await db.ExecuteScalarAsync<int>("SELECT Count(*) FROM Practice");

                return count;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        #endregion

        #region Schedule
        /// <summary>
        /// Speichert oder updatet einen Trainingsplan in der lokalen DB
        /// </summary>
        /// <param name="data">Schedule</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>0 -> Update</returns>
        /// <returns>1 -> Insert</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> insertUpdateSchedule(ScheduleModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                if (await db.InsertAsync(data) != 0)
                {
                    await db.UpdateAsync(data);
                    return 0;
                }
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Löscht einen Trainingsplan aus der lokalen DB
        /// </summary>
        /// <param name="data">Schedule</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>1 -> Delete</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> deleteSchedule(ScheduleModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.DeleteAsync(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Sucht einen Trainingsplan in der lokalen DB
        /// </summary>
        /// <param name="data">Schedule</param>
        /// <param name="path"></param>
        /// <returns>1 -> Vorhanden</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findSchedule(ScheduleModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.FindAsync<ScheduleModel>(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Gibt die Anzahl der gespeicherten Trainingspläne der lokalen DB zurück
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Anzahl der Trainingspläne</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findNumberOfSchedules()
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                var count = await db.ExecuteScalarAsync<int>("SELECT Count(*) FROM Schedule");

                return count;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Gibt einen Trainingsplan an Hand der ID zurück
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ScheduleModel> GetScheduleByIdAsync(int id)
        {
            return new ScheduleModel();
        }

        /// <summary>
        /// Gibt alle Trainingspläne eines Users zurück
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ScheduleModel>> GetAllSchedulesAsync(int userID)
        {
            return null;
        }
        #endregion

        #region Exercise
        /// <summary>
        /// Speichert oder updatet eine Übung in der lokalen DB
        /// </summary>
        /// <param name="data">Exercise</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>0 -> Update</returns>
        /// <returns>1 -> Insert</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> insertUpdateExercise(ExerciseModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                if (await db.InsertAsync(data) != 0)
                {
                    await db.UpdateAsync(data);
                    return 0;
                }
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Löscht eine Übung aus der lokalen DB
        /// </summary>
        /// <param name="data">Exercise</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>1 -> Delete</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> deleteExercise(ExerciseModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.DeleteAsync(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Sucht eine Übung in der lokalen DB
        /// </summary>
        /// <param name="data">Exercise</param>
        /// <param name="path"></param>
        /// <returns>1 -> Vorhanden</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findExercise(ExerciseModel data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.FindAsync<ExerciseModel>(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Gibt eine Übung an Hand der ID zurück
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <returns></returns>
        public async Task<ExerciseModel> GetExerciseByIdAsync(int exerciseId)
        {
            return new ExerciseModel();
        }

        /// <summary>
        /// Gibt die Anzahl der gespeicherten Übungen der lokalen DB zurück
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Anzahl der Übungen</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findNumberOfExercises()
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                var count = await db.ExecuteScalarAsync<int>("SELECT Count(*) FROM Exercise");

                return count;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        #endregion
    }
}