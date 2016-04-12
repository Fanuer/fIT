using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using fITNat.DBModels;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinAndroid;

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
        using (var con = LocalDB.CreateCon())
        {
          bool test = con.StoreDateTimeAsTicks;
          var user = con.CreateTable<User>();
          var practice = con.CreateTable<Practice>();
          var schedule = con.CreateTable<Schedule>();
          var exercise = con.CreateTable<Exercise>();
          var sHe = con.CreateTable<ScheduleHasExercises>();
        }
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
        using (var db = LocalDB.CreateCon())
        {
          if (db.Insert(data) != 0)
          {
            db.Update(data);
            return 0;
          }
          return 1;
        }
      }
      catch (SQLiteException ex)
      {
        Console.WriteLine(ex.Message);
        return -1;
      }
    }

    /// <summary>
    /// Gibt einen User anhand seiner userId zurück
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public User findUser(string userId)
    {
      User result = null;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          List<User> t = db.Query<User>("Select * From User Where UserId=?", userId);
          User user = t.First<User>();
          if (user != null)
          {
            result = user;
          }
        }
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
        using (var db = LocalDB.CreateCon())
        {
          List<User> result = db.Query<User>("Select * From User Where username=?", username);
          User user = result.First<User>();
          if (user.Password == password)
          {
            return Guid.Parse(user.UserId);
          }
        }
      }
      catch (SQLiteException ex)
      {
        Console.WriteLine(ex.Message);
      }
      return new Guid();
    }

    /*
    /// <summary>
    /// Gibt für die Methode OnOffService.checkSync() alle User zurück, die im Offline-Modus angelegt wurden
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
      List<Schedule> tempList;
      Schedule temp;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          tempList = db.Query<Schedule>("Select * From Schedule Where Id=?", data.Id);
          if (tempList.Count != 0)
          {
            temp = tempList.First<Schedule>();
            //Bei temp alles updaten außer Id und LocalId und dann ein Update darauf fahren
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
        using (var db = LocalDB.CreateCon())
        {
          db.Delete(data);
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
    /// Gibt einen Trainingsplan anhand der Id aus der lokalen DB zurück
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Schedule GetScheduleById(int id)
    {
      Schedule result = null;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          List<Schedule> schedules = db.Query<Schedule>("Select * From Schedule Where id=?", id);
          Schedule schedule = schedules.First<Schedule>();
          if (schedule != null)
          {
            result = schedule;
          }
        }
      }
      catch (SQLiteException ex)
      {
        Console.WriteLine(ex.Message);
      }
      return result;
    }

    /// <summary>
    /// Gibt alle Trainingspläne eines Users aus der lokalen DB zurück
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public IEnumerable<Schedule> GetAllSchedules(Guid userId)
    {
      IEnumerable<Schedule> result = null;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          List<Schedule> schedules = db.Query<Schedule>("Select * From Schedule Where UserId=?", userId.ToString());

          if (schedules != null)
          {
            result = schedules;
          }
        }
      }
      catch (SQLiteException ex)
      {
        Console.WriteLine(ex.Message);
      }
      return result;
    }

    /// <summary>
    /// Gibt alle zugehörigen Trainingspläne zu einer Übung zurück (wird zum Anlegen eines ExerciseModel benötigt)
    /// </summary>
    /// <param name="exerciseId"></param>
    /// <returns></returns>
    public IEnumerable<int> GetAllSchedulesByExercise(int exerciseId)
    {
      IEnumerable<int> result = null;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          List<int> schedules = db.Query<string>("Select ScheduleId From ScheduleHasExercises Where ExerciseId = ?", exerciseId).Select(x => Convert.ToInt32(x)).ToList();
          if (schedules != null)
          {
            result = schedules;
          }
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
      int result;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          ScheduleHasExercises sHe = new ScheduleHasExercises();
          sHe.ExerciseId = exerciseId;
          sHe.ScheduleId = scheduleId;
          sHe.WasOffline = wasOffline;
          List<ScheduleHasExercises> bestand = db.Query<ScheduleHasExercises>("Select * From ScheduleHasExercises Where ScheduleId=?", scheduleId);
          if (bestand.Count != 0)
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
    /// Speichert oder updatet eine Übung in der lokalen DB
    /// </summary>
    /// <param name="data">Exercise</param>
    /// <param name="path">SQLite Connection String</param>
    /// <returns>0 -> Update</returns>
    /// <returns>1 -> Insert</returns>
    /// <returns>-1 -> Exception</returns>
    public int insertUpdateExercise(Exercise data)
    {
      List<Exercise> tempList;
      Exercise temp;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          tempList = db.Query<Exercise>("Select * From Exercise Where Id=?", data.Id);
          if (tempList.Count != 0)
          {
            temp = tempList.First<Exercise>();
            //Bei temp alles updaten außer Id und LocalId und dann ein Update darauf fahren
            temp.Name = data.Name;
            temp.Url = data.Url;
            temp.Description = data.Description;
            temp.WasOffline = false;
            db.Update(temp);
            return 0;
          }
          db.Insert(data);
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
    /// Gibt eine Exercise anhand der Id aus der lokalen DB zurück
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Exercise GetExerciseById(int id)
    {
      Exercise result = null;
      List<Exercise> exercises;
      Exercise exercise;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          exercises = db.Query<Exercise>("Select * From Exercise Where Id=?", id);
          exercise = exercises.First<Exercise>();
          if (exercise != null)
          {
            result = exercise;
          }

        }
      }
      catch (SQLiteException ex)
      {
        Console.WriteLine(ex.Message);
      }
      return result;
    }

    /// <summary>
    /// Gibt alle Übungen zu einem Trainingsplan aus der lokalen DB zurück
    /// </summary>
    /// <param name="scheduleID"></param>
    /// <returns></returns>
    public List<Exercise> GetExercisesOfSchedule(int scheduleID)
    {
      List<ScheduleHasExercises> scheduleHasExercises = null;
      List<Exercise> result = new List<Exercise>();
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          scheduleHasExercises = db.Query<ScheduleHasExercises>("Select * From ScheduleHasExercises Where ScheduleId = ?", scheduleID);
          if (scheduleHasExercises.Count != 0)
          {
            result.AddRange(scheduleHasExercises.Select(sHE => GetExerciseById(sHE.ExerciseId)));
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
    /// Gibt alle Übungen zu einem Trainingsplan aus der lokalen DB zurück (benötigt für Anlegen eines ScheduleModel)
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
      List<Practice> tempList;
      Practice temp;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          tempList = db.Query<Practice>("Select * From Practice Where LocalId=?", data.LocalId);
          if (tempList.Count != 0)
          {
            temp = tempList.First<Practice>();
            //Bei temp alles updaten außer Id und LocalId und dann ein Update darauf fahren
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
    /// Speichert oder updatet ein Training in der lokalen DB vom Online-Status
    /// </summary>
    /// <param name="data">Prcatice</param>
    /// <param name="path">SQLite Connection String</param>
    /// <returns>0 -> Update</returns>
    /// <returns>1 -> Insert</returns>
    /// <returns>-1 -> Exception</returns>
    public int insertUpdatePracticeOnline(Practice data)
    {
      List<Practice> tempList;
      Practice temp;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          tempList = db.Query<Practice>("Select * From Practice Where Id=?", data.Id);
          if (tempList.Count != 0)
          {
            temp = tempList.First<Practice>();
            //Bei temp alles updaten außer LocalId und dann ein Update darauf fahren
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

        }
        using (var db2 = LocalDB.CreateCon())
        {
          List<Practice> testListe = db2.Query<Practice>("Select * From Practice Where LocalId=?", data.LocalId);
          Practice test = testListe.First<Practice>();
          Console.WriteLine(test.LocalId);
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
    public int deletePractice(Practice data)
    {
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          db.Delete(data);
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
    /// Gibt ein Training anhand der Id zurück
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Practice GetPracticeById(int id)
    {
      Practice result = null;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          List<Practice> practices = db.Query<Practice>("Select * From Practice Where Id=?", id);
          Practice practice = practices.First<Practice>();
          if (practice != null)
          {
            result = practice;
          }
        }
      }
      catch (SQLiteException ex)
      {
        Console.WriteLine(ex.Message);
      }
      return result;
    }

    /// <summary>
    /// Gibt alle Trainings zurück, die Offline angelegt wurden
    /// </summary>
    /// <returns>Liste mit Offline-Trainings</returns>
    public List<Practice> GetOfflinePractice()
    {
      List<Practice> result = null;
      try
      {
        using (var db = LocalDB.CreateCon()) {
          result = db.Query<Practice>("Select * From Practice Where wasOffline=?", true);
        }
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
      List<Practice> practice;
      Practice p;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          practice = db.Query<Practice>("Select * From Practice Where LocalId=?", data.LocalId);
          List<Practice> res = db.Query<Practice>("Update Practice Set WasOffline='false' Where LocalId=?", data.LocalId);
          p = practice.First<Practice>();
          p.WasOffline = false;
          return db.Update(p, typeof (Practice)) != -1;
        }

      }
      catch (SQLiteException ex)
      {
        Console.WriteLine(ex.Message);
      }
      return false;
    }


    /// <summary>
    /// Gibt alle Trainings zu einem User mit dem Trainingsplan und der Übung zurück
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="scheduleId"></param>
    /// <param name="exerciseId"></param>
    /// <returns></returns>
    public List<Practice> GetAllPracticesByUserScheduleExercise(string userId, int scheduleId, int exerciseId)
    {
      List<Practice> result = new List<Practice>();
      List<Practice> test = null;
      try
      {
        using (var db = LocalDB.CreateCon())
        {
          test = db.Query<Practice>("Select * From Practice Where ScheduleId=?", scheduleId);
        }
        result.AddRange(test.Where(item => item.ExerciseId == exerciseId));
      }
      catch (SQLiteException ex)
      {
        Console.WriteLine(ex.Message);
      }
      return result;
    }
    #endregion

    #region  private

    private static SQLiteConnection CreateCon()
    {
      return new SQLiteConnection(new SQLitePlatformAndroid(), LocalDB.path);
    }
    #endregion
  }
}