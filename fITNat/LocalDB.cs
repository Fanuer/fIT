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

namespace fITNat
{
    class LocalDB
    {
        public async Task<bool> createDatabase(string path)
        {
            try
            {
                var connection = new SQLiteAsyncConnection(path);
                {
                    await connection.CreateTableAsync<UserModel>();
                    await connection.CreateTableAsync<PracticeModel>();
                    await connection.CreateTableAsync<ScheduleModel>();
                    await connection.CreateTableAsync<ExerciseModel>();
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Fehler beim Anlegen der DB: " + ex.StackTrace);
                return false;
            }
        }

        #region User
        /// <summary>
        /// Speichert oder updatet einen User in der lokalen DB
        /// </summary>
        /// <param name="data">User</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>0 -> Update</returns>
        /// <returns>1 -> Insert</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> insertUpdateUser(UserModel data, string path)
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
        /// L�scht einen User aus der lokalen DB
        /// </summary>
        /// <param name="data">User</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>1 -> Delete</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> deleteUser(UserModel data, string path)
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
        public async Task<int> findUser(UserModel data, string path)
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
        /// Gibt die Anzahl der gespeicherten User der lokalen DB zur�ck
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Anzahl der User</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findNumberOfUsers(string path)
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
        public async Task<int> insertUpdatePractice(PracticeModel data, string path)
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
        /// L�scht ein Training aus der lokalen DB
        /// </summary>
        /// <param name="data">Practice</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>1 -> Delete</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> deletePractice(PracticeModel data, string path)
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
        public async Task<int> findPractice(PracticeModel data, string path)
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
        /// Gibt die Anzahl der gespeicherten Trainings der lokalen DB zur�ck
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Anzahl der Trainings</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findNumberOfPractices(string path)
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
        public async Task<int> insertUpdateSchedule(ScheduleModel data, string path)
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
        /// L�scht einen Trainingsplan aus der lokalen DB
        /// </summary>
        /// <param name="data">Schedule</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>1 -> Delete</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> deleteSchedule(ScheduleModel data, string path)
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
        public async Task<int> findSchedule(ScheduleModel data, string path)
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
        /// Gibt die Anzahl der gespeicherten Trainingspl�ne der lokalen DB zur�ck
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Anzahl der Trainingspl�ne</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findNumberOfSchedules(string path)
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
        #endregion

        #region Exercise
        /// <summary>
        /// Speichert oder updatet eine �bung in der lokalen DB
        /// </summary>
        /// <param name="data">Exercise</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>0 -> Update</returns>
        /// <returns>1 -> Insert</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> insertUpdateExercise(ExerciseModel data, string path)
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
        /// L�scht eine �bung aus der lokalen DB
        /// </summary>
        /// <param name="data">Exercise</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>1 -> Delete</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> deleteExercise(ExerciseModel data, string path)
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
        /// Sucht eine �bung in der lokalen DB
        /// </summary>
        /// <param name="data">Exercise</param>
        /// <param name="path"></param>
        /// <returns>1 -> Vorhanden</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findExercise(ExerciseModel data, string path)
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
        /// Gibt die Anzahl der gespeicherten �bungen der lokalen DB zur�ck
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Anzahl der �bungen</returns>
        /// <returns>-1 -> Exception</returns>
        public async Task<int> findNumberOfExercises(string path)
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