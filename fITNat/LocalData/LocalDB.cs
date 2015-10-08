using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fITNat.DBModels;

namespace fITNat.LocalData
{
    public class LocalDB
    {
        private static readonly string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private static readonly string path = System.IO.Path.Combine(folder, "app.sqlite");

        /// <summary>
        /// Erstellt die lokale Datenbank aus den DBModels
        /// </summary>
        public static void createDatabase()
        {
            try
            {
                var con2 = new SQLiteConnection(path);
                bool test = con2.StoreDateTimeAsTicks;
                var user = con2.CreateTable<User>();
                var practice = con2.CreateTable<Practice>();
                var schedule = con2.CreateTable<Schedule>();
                var exercise = con2.CreateTable<Exercise>();
                var sHe = con2.CreateTable<ScheduleHasExercises>();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Fehler beim Anlegen der DB: " + ex.StackTrace);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Fehler beim Anlegen der DB: " + exc.StackTrace);
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
        public int insertUpdateUser(User data)
        {
            try
            {
                var db = new SQLiteConnection(path);
                if (db.Insert(data) != 0)
                {
                    db.Update(data);
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
        /// Gibt einen User anhand seiner userId zur�ck
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User findUser(string userId)
        {
            User result = null;
            try
            {
                var db = new SQLiteConnection(path);
                List<User> t = db.Query<User>("Select * From User Where UserId=?", userId);
                User user = t.First<User>();
                if (user != null)
                    result = user;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Sucht einen User anhand von Username und PW in der lokalen DB
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Guid findUser(string username, string password)
        {
            try
            {
                var db = new SQLiteConnection(path);
                List<User> result = db.Query<User>("Select * From User Where username=?", username);
                User user = result.First<User>();
                if (user.Password == password)
                    return Guid.Parse(user.UserId);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new Guid();
        }

        /*
        /// <summary>
        /// Gibt f�r die Methode OnOffService.checkSync() alle User zur�ck, die im Offline-Modus angelegt wurden
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> getOfflineUser()
        {
            List<User> result = null;
            try
            {
                var db = new SQLiteConnection(path);
                result = db.Query<User>("Select * From User Where wasOffline=?", true);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }*/
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
        public int insertUpdateSchedule(Schedule data)
        {
            SQLiteConnection db;
            List<Schedule> tempList;
            Schedule temp;
            try
            {
                db = new SQLiteConnection(path);
                tempList = db.Query<Schedule>("Select * From Schedule Where Id=?", data.Id);
                if(tempList.Count != 0)
                {
                    temp = tempList.First<Schedule>();
                    //Bei temp alles updaten au�er Id und LocalId und dann ein Update darauf fahren
                    temp.Name = data.Name;
                    temp.Url = data.Url;
                    temp.UserId = data.UserId;
                    temp.WasOffline = false;
                    db.Update(temp);
                    return 0;
                }
                db.Insert(data);
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
        /// Gibt einen Trainingsplan anhand der Id aus der lokalen DB zur�ck
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Schedule GetScheduleById(int id)
        {
            Schedule result = null;
            try
            {
                var db = new SQLiteConnection(path);
                List<Schedule> schedules = db.Query<Schedule>("Select * From Schedule Where id=?", id);
                Schedule schedule = schedules.First<Schedule>();
                if (schedule != null)
                {
                    result = schedule;
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Gibt alle Trainingspl�ne eines Users aus der lokalen DB zur�ck
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Schedule> GetAllSchedules(Guid userId)
        {
            IEnumerable<Schedule> result = null;
            try
            {
                var db = new SQLiteConnection(path);
                List<Schedule> schedules = db.Query<Schedule>("Select * From Schedule Where UserId=?", userId.ToString());
                if (schedules != null)
                {
                    result = schedules;
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Gibt alle zugeh�rigen Trainingspl�ne zu einer �bung zur�ck (wird zum Anlegen eines ExerciseModel ben�tigt)
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <returns></returns>
        public IEnumerable<int> GetAllSchedulesByExercise(int exerciseId)
        {
            IEnumerable<int> result = null;
            try
            {
                var db = new SQLiteConnection(path);
                List<int> schedules = db.Query<int>("Select ScheduleId From ScheduleHasExercises Where ExerciseId = ?", exerciseId);
                if (schedules != null)
                {
                    result = schedules;
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public bool InsertScheduleHasExercises(int scheduleId, int exerciseId, bool wasOffline)
        {
            SQLiteConnection db;
            int result;
            try
            {
                db = new SQLiteConnection(path);
                ScheduleHasExercises sHe = new ScheduleHasExercises();
                sHe.ExerciseId = exerciseId;
                sHe.ScheduleId = scheduleId;
                sHe.WasOffline = wasOffline;
                List<ScheduleHasExercises> bestand = db.Query<ScheduleHasExercises>("Select * From ScheduleHasExercises Where ScheduleId=?", scheduleId);
                if(bestand.Count != 0)
                {
                    foreach (var data in bestand)
                    {
                        if (data.ExerciseId == sHe.ExerciseId && data.ScheduleId == sHe.ScheduleId)
                        {
                            return true;
                        }
                        else
                        {
                            result = db.Insert(sHe, typeof(ScheduleHasExercises));
                            if (result != -1)
                                return true;
                        }
                    }
                }
                else
                {
                    result = db.Insert(sHe, typeof(ScheduleHasExercises));
                    if (result != -1)
                        return true;
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
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
        public int insertUpdateExercise(Exercise data)
        {
            SQLiteConnection db;
            List<Exercise> tempList;
            Exercise temp;
            try
            {
                db = new SQLiteConnection(path);
                tempList = db.Query<Exercise>("Select * From Exercise Where Id=?", data.Id);
                if (tempList.Count != 0)
                {
                    temp = tempList.First<Exercise>();
                    //Bei temp alles updaten au�er Id und LocalId und dann ein Update darauf fahren
                    temp.Name = data.Name;
                    temp.Url = data.Url;
                    temp.Description = data.Description;
                    temp.WasOffline = false;
                    db.Update(temp);
                    return 0;
                }
                db.Insert(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Gibt eine Exercise anhand der Id aus der lokalen DB zur�ck
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Exercise GetExerciseById(int id)
        {
            Exercise result = null;
            SQLiteConnection db;
            List<Exercise> exercises;
            Exercise exercise;
            try
            {
                db = new SQLiteConnection(path);
                exercises = db.Query<Exercise>("Select * From Exercise Where Id=?", id);
                exercise = exercises.First<Exercise>();
                if (exercise != null)
                {
                    result = exercise;
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Gibt alle �bungen zu einem Trainingsplan aus der lokalen DB zur�ck
        /// </summary>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        public List<Exercise> GetExercisesOfSchedule(int scheduleID)
        {
            List<ScheduleHasExercises> scheduleHasExercises = null;
            SQLiteConnection db;
            List<Exercise> result = new List<Exercise>();
            try
            {
                db = new SQLiteConnection(path);
                scheduleHasExercises = db.Query<ScheduleHasExercises>("Select * From ScheduleHasExercises Where ScheduleId = ?", scheduleID);
                if (scheduleHasExercises.Count != 0)
                {
                    foreach (var sHE in scheduleHasExercises)
                    {
                        Exercise exercise = GetExerciseById(sHE.ExerciseId);
                        result.Add(exercise);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        /*
        /// <summary>
        /// Gibt alle �bungen zu einem Trainingsplan aus der lokalen DB zur�ck (ben�tigt f�r Anlegen eines ScheduleModel)
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        public IEnumerable<int> GetAllExercisesBySchedule(int scheduleId)
        {
            IEnumerable<int> result = null;
            try
            {
                var db = new SQLiteConnection(path);
                List<int> exercises = db.Query<int>("Select ExerciseId From ScheduleHasExercises Where ScheduleId = ?", scheduleId);
                if (exercises != null)
                {
                    result = exercises;
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        */
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
        public int insertUpdatePracticeOffline(Practice data)
        {
            SQLiteConnection db;
            List<Practice> tempList;
            Practice temp;
            try
            {
                db = new SQLiteConnection(path);
                tempList = db.Query<Practice>("Select * From Practice Where LocalId=?", data.LocalId);
                if (tempList.Count != 0)
                {
                    temp = tempList.First<Practice>();
                    //Bei temp alles updaten au�er Id und LocalId und dann ein Update darauf fahren
                    temp.ExerciseId = data.ExerciseId;
                    temp.NumberOfRepetitions = data.NumberOfRepetitions;
                    temp.Repetitions = data.Repetitions;
                    temp.ScheduleId = data.ScheduleId;
                    temp.Timestamp = data.Timestamp;
                    temp.Url = data.Url;
                    temp.UserId = data.UserId;
                    temp.Weight = data.Weight;
                    temp.WasOffline = data.WasOffline;
                    db.Update(temp);
                    return 0;
                }
                db.Insert(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Speichert oder updatet ein Training in der lokalen DB vom Online-Status
        /// </summary>
        /// <param name="data">Prcatice</param>
        /// <param name="path">SQLite Connection String</param>
        /// <returns>0 -> Update</returns>
        /// <returns>1 -> Insert</returns>
        /// <returns>-1 -> Exception</returns>
        public int insertUpdatePracticeOnline(Practice data)
        {
            SQLiteConnection db;
            List<Practice> tempList;
            Practice temp;
            try
            {
                db = new SQLiteConnection(path);
                tempList = db.Query<Practice>("Select * From Practice Where Id=?", data.Id);
                if (tempList.Count != 0)
                {
                    temp = tempList.First<Practice>();
                    //Bei temp alles updaten au�er LocalId und dann ein Update darauf fahren
                    temp.ExerciseId = data.ExerciseId;
                    temp.NumberOfRepetitions = data.NumberOfRepetitions;
                    temp.Repetitions = data.Repetitions;
                    temp.ScheduleId = data.ScheduleId;
                    temp.Timestamp = data.Timestamp;
                    temp.Url = data.Url;
                    temp.UserId = data.UserId;
                    temp.Weight = data.Weight;
                    temp.Id = data.Id;
                    temp.WasOffline = false;
                    db.Update(temp);
                    return 0;
                }
                db.Insert(data);
                List<Practice> testListe = db.Query<Practice>("Select * From Practice Where LocalId=?", data.LocalId);
                Practice test = testListe.First<Practice>();
                Console.WriteLine(test.LocalId);
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
        public int deletePractice(Practice data)
        {
            try
            {
                var db = new SQLiteConnection(path);
                db.Delete(data);
                return 1;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Gibt ein Training anhand der Id zur�ck
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Practice GetPracticeById(int id)
        {
            Practice result = null;
            try
            {
                var db = new SQLiteConnection(path);
                List<Practice> practices = db.Query<Practice>("Select * From Practice Where Id=?", id);
                Practice practice = practices.First<Practice>();
                if (practice != null)
                {
                    result = practice;
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Gibt alle Trainings zur�ck, die Offline angelegt wurden
        /// </summary>
        /// <returns>Liste mit Offline-Trainings</returns>
        public List<Practice> GetOfflinePractice()
        {
            List<Practice> result = null;
            SQLiteConnection db;
            try
            {
                db = new SQLiteConnection(path);
                result = db.Query<Practice>("Select * From Practice Where wasOffline=?", true);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Setzt den Status des Trainings auf Online
        /// </summary>
        /// <param name="data"></param>
        public bool setPracticeOnline(Practice data)
        {
            SQLiteConnection db;
            List<Practice> practice;
            Practice p;
            try
            {
                db = new SQLiteConnection(path);
                practice = db.Query<Practice>("Select * From Practice Where LocalId=?", data.LocalId);
                List<Practice> res = db.Query<Practice>("Update Practice Set WasOffline='false' Where LocalId=?", data.LocalId);
                p = practice.First<Practice>();
                p.WasOffline = false;
                if (db.Update(p, typeof(Practice)) != -1)
                    return true;
                else
                    return false;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        
        /// <summary>
        /// Gibt alle Trainings zu einem User mit dem Trainingsplan und der �bung zur�ck
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="scheduleId"></param>
        /// <param name="exerciseId"></param>
        /// <returns></returns>
        public List<Practice> GetAllPracticesByUserScheduleExercise(string userId, int scheduleId, int exerciseId)
        {
            List<Practice> result = new List<Practice>();
            try
            {
                var db = new SQLiteConnection(path);
                List<Practice> test = db.Query<Practice>("Select * From Practice Where ScheduleId=?", scheduleId);
                foreach (var item in test)
                {
                    if(item.ExerciseId == exerciseId)
                    {
                        result.Add(item);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        #endregion
    }
}