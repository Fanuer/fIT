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
using fITNat.DBModels;

namespace fITNat
{
    class LocalDB
    {
        private static readonly string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private static readonly string path = System.IO.Path.Combine(folder, "app.db");

        /// <summary>
        /// Legt die Datenbank lokal mit allen Tabellen an
        /// </summary>
        /// <returns></returns>
        public async Task<bool> createDatabase()
        {
            try
            {
                var connection = new SQLiteAsyncConnection(path);
                await connection.CreateTableAsync<User>();
                //Console.WriteLine("User geht");
                await connection.CreateTableAsync<Schedule>();
                //Console.WriteLine("Schedule geht");
                await connection.CreateTableAsync<ScheduleHasExercises>();
                await connection.CreateTableAsync<Exercise>();
                //Console.WriteLine("Exercise geht");
                await connection.CreateTableAsync<Practice>();
                await CreateTestdata();
                Console.WriteLine("Alles geht");
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

        private async Task CreateTestdata()
        {
            //User
            User test = new User();
            test.Password = "Test1234";
            test.Username = "Kevin";
            User testa = new User();
            testa.Password = "Test1234";
            testa.Username = "Stefan";
            await insertUpdateUser(test);
            await insertUpdateUser(testa);
            //Schedules
            Schedule test1 = new Schedule();
            test1.Id = 1;
            test1.Name = "Push";
            test1.Url = "testtest";
            test1.UserId = "Kevin";
            await insertUpdateSchedule(test1);
            Schedule test2 = new Schedule();
            test2.Id = 2;
            test2.Name = "Pull";
            test2.Url = "test2test2";
            test2.UserId = "Kevin";
            await insertUpdateSchedule(test2);
            Schedule test3 = new Schedule();
            test3.Id = 2;
            test3.Name = "Beine";
            test3.Url = "test2test2";
            test3.UserId = "Kevin";
            await insertUpdateSchedule(test3);
            Schedule test4 = new Schedule();
            test4.Id = 2;
            test4.Name = "Pull";
            test4.Url = "test2test2";
            test4.UserId = "Stefan";
            await insertUpdateSchedule(test4);
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
        public async Task<int> insertUpdateUser(User data)
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
        public async Task<int> deleteUser(User data)
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
        public async Task<int> findUser(User data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.FindAsync<User>(data);
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
        public async Task<List<User>> findUser(string username, string password)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                var result = await db.QueryAsync<User>("Select * From User Where Username=?", username);
                
                return result;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
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
        public async Task<int> insertUpdatePractice(Practice data)
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

        public async Task<int> insertPractice(string userId, int scheduleId, int exerciseId, DateTime timestamp = default(DateTime),
                                        double weight = 0, int repetitions = 0, int numberOfRepetitions = 0)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                string query = "Insert into Practice (Timestamp, Weight, NumberOfRepetitions, Repetitions, ScheduleId, ExerciseId, UserId) values (" + timestamp + ", " + weight + ", " + numberOfRepetitions + ", " + repetitions + ", " + scheduleId + ", " + exerciseId + ", " + userId + ")";
                var result = await db.QueryAsync<int>(query, null);
                if (result != null)
                    return 1;
            }
            catch(SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// Löscht ein Training aus der lokalen DB
        /// </summary>
        /// <param name="data">Practice</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>1 -> Delete</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> deletePractice(Practice data)
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
        public async Task<int> findPractice(Practice data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.FindAsync<Practice>(data);
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
        public async Task<int> insertUpdateSchedule(Schedule data)
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
        public async Task<int> deleteSchedule(Schedule data)
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
        public async Task<int> findSchedule(Schedule data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.FindAsync<Schedule>(data);
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
        public async Task<Schedule> GetScheduleByIdAsync(int id)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                var result = await db.QueryAsync<Schedule>("select * from Schedule where Id=?", id);
                return result.First<Schedule>();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gibt die Verbindungen zwischen Schedules und Exercises aus der Tabelle anhand der ScheduleId zurück
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ScheduleHasExercises>> GetExercisesOfSchedule(int scheduleId)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                var result = await db.QueryAsync<ScheduleHasExercises>("Select * From ScheduleHasExercises Where ScheduleId = ?", scheduleId);
                return result;
            }
            catch(SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gibt alle Trainingspläne eines Users zurück
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync(Guid userID)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                IEnumerable<Schedule> result = await db.QueryAsync<Schedule>("select * from Schedule where UserId=?", userID);
                return result;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
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
        public async Task<int> insertUpdateExercise(Exercise data)
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
        public async Task<int> deleteExercise(Exercise data)
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
        public async Task<int> findExercise(Exercise data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                await db.FindAsync<Exercise>(data);
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
        public async Task<Exercise> GetExerciseByIdAsync(int exerciseId)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                var result = await db.QueryAsync<Exercise>("select * from Exercise where Id=?", exerciseId);
                return result.First<Exercise>();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
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