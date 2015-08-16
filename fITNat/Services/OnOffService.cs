using System;
using System.Collections.Generic;

using fITNat.DBModels;
using Android.App;
using Android.Content;
using Android.OS;
using fIT.WebApi.Client.Data.Models.Shared.Enums;
using System.Threading.Tasks;
using fITNat.Services;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fIT.WebApi.Client.Data.Models.Schedule;
using fIT.WebApi.Client.Data.Models.Exercise;
using fIT.WebApi.Client.Portable.Implementation;
using fIT.WebApi.Client.Data.Models.Shared;
using System.Threading;

namespace fITNat
{
    [Service]
    public class OnOffService : Service
    {
        #region Variablen
        public static ManagementServiceLocal mgnService { get; private set; }
        private ManagementService mgnServiceServer;
        private const string URL = @"http://fit-bachelor.azurewebsites.net/";
        public static LocalDB db { get; private set; }
        public static bool Online { get; private set; }
        private Object thisLock = new Object();
        private IBinder mBinder;
        #endregion

        public static void setzeManagementServiceLocal(ManagementServiceLocal data)
        {
            mgnService = data;
        }

        public static void setzeStatus(bool data)
        {
            Online = data;
        }

        public static void setzeDb(LocalDB data)
        {
            db = data;
        }

        //OnCreate -> Initialisierung
        public override void OnCreate()
        {
            base.OnCreate(); 
            mgnServiceServer = new ManagementService(URL);
            setzeManagementServiceLocal(new ManagementServiceLocal());
            setzeDb(new LocalDB());
            ThreadStart myThreadDelegate = new ThreadStart(LocalDB.createDatabase);
            Thread myThread = new Thread(myThreadDelegate);
            myThread.Start();
            //LocalDB.createDatabase();
            myThread.Join();
            Console.WriteLine("DB erstellt");
        }

        #region User
        /// <summary>
        /// Entsprechend des Netzwerkstatus wird lokal oder am Server angelegt
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Guid> SignIn(string username, string password)
        {
            User user = new fITNat.DBModels.User();
            Guid userId;
            user.Username = username;
            user.Password = password;
            Console.WriteLine("Online vor der Entscheidung: " + Online);
            if (Online)
            {
                try
                {
                    bool success = await mgnService.SignIn(username, password);
                    if(success)
                    {
                        userId = mgnService.actualSession().CurrentUserId;
                        user.UserId = userId.ToString();
                        if(db != null)
                        {
                            db.insertUpdateUser(user);
                        }
                        System.Console.WriteLine("Online eingeloggt");
                        return userId;
                    }
                }
                catch(ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Online-Einloggen (Server)" + ex.StackTrace);
                    throw;
                }
                catch(Exception exc)
                {
                    System.Console.WriteLine("Fehler beim Online-Einloggen" + exc.StackTrace);
                }
            }
            else
            {
                try
                {
                    //Lokal nachgucken
                    Guid result = db.findUser(username, password);
                    if (result != null)
                        return result;
                }
                catch(Exception exc)
                {
                    System.Console.WriteLine("Fehler beim Offline-Einloggen" + exc.StackTrace);
                }
            }
            return new Guid();
        }

        /// <summary>
        /// Entsprechend des Netzwerkstatus wird am Server angelegt oder ein Fehler angezeigt
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="passwordConfirm"></param>
        /// <param name="gender"></param>
        /// <param name="job"></param>
        /// <param name="fitness"></param>
        /// <param name="birthdate"></param>
        /// <returns></returns>
        public async Task<bool> SignUp( string username,
                                        string email,
                                        string password,
                                        string passwordConfirm,
                                        GenderType gender,
                                        JobTypes job,
                                        FitnessType fitness,
                                        DateTime birthdate)
        {
            if (Online)
            {
                try
                {
                    await mgnService.SignUp(username, email, password, passwordConfirm, gender, job, fitness, birthdate);
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Registrieren: " + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                //Fehler schmeissen, dass man offline ist!!!
                throw (new Exception("Registrierung funktioniert nur im Online-Modus"));
            }
            return true;
        }
        #endregion

        #region Practice
        /// <summary>
        /// Entsprechend des Netzwerkstatus wird lokal oder am Server angelegt
        /// </summary>
        /// <param name="id"></param>
        /// <param name="scheduleId"></param>
        /// <param name="exerciseId"></param>
        /// <param name="userId"></param>
        /// <param name="timestamp"></param>
        /// <param name="weight"></param>
        /// <param name="repetitions"></param>
        /// <param name="numberOfRepetitions"></param>
        /// <returns></returns>
        public async Task<bool> createPractice( int scheduleId,
                                                int exerciseId,
                                                string userId,
                                                DateTime timestamp = default(DateTime),
                                                double weight = 0,
                                                int repetitions = 0,
                                                int numberOfRepetitions = 0)
        {
            if (Online)
            {
                try
                {
                    if (await mgnService.recordPractice(scheduleId, exerciseId, timestamp, weight, repetitions, numberOfRepetitions))
                    {
                        Practice p = new Practice();
                        p.UserId = userId;
                        p.ScheduleId = scheduleId;
                        p.ExerciseId = exerciseId;
                        p.Timestamp = timestamp;
                        p.Weight = weight;
                        p.Repetitions = repetitions;
                        p.NumberOfRepetitions = numberOfRepetitions;
                        int result = db.insertUpdatePractice(p);
                        if(result == 1)
                        {
                            Console.WriteLine("Training auch lokal angelegt");
                        }
                        return true;
                    }
                    else
                        return false;
                }
                catch(ServerException ex)
                {
                    throw;
                }
            }
            else
            {
                try
                {
                    Practice p = new Practice();
                    p.UserId = userId;
                    p.ScheduleId = scheduleId;
                    p.ExerciseId = exerciseId;
                    p.Timestamp = timestamp;
                    p.Weight = weight;
                    p.Repetitions = repetitions;
                    p.NumberOfRepetitions = numberOfRepetitions;
                    int result = db.insertUpdatePractice(p);
                    if (result == 1)
                        return true;
                    else
                        return false;
                }
                catch(Exception exc)
                {
                    System.Console.WriteLine("Fehler beim lokalen Anlegen des Trainings: " + exc.StackTrace);
                }
            }
            return true;
        }
        #endregion

        #region Schedule
        /// <summary>
        /// Gibt alle Trainingspl�ne eines Benutzers zur�ck
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ScheduleModel>> GetAllSchedulesAsync(Guid userID = new Guid())
        {
            if (Online)
            {
                try
                {
                    IEnumerable<ScheduleModel> schedules = await mgnService.GetAllSchedulesAsync();
                    if(schedules != null)
                    {
                        return schedules;
                    }
                    return null;
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Abrufen der Trainingspl�ne: " + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                try
                {
                    //Alle Trainingspl�ne f�r den User zur�ckgeben
                    IEnumerable<Schedule> schedules = db.GetAllSchedules(userID);
                    List<ScheduleModel> resultSchedules = new List<ScheduleModel>();
                    if(schedules != null)
                    {
                        foreach (var schedule in schedules)
                        {
                            ScheduleModel temp = new ScheduleModel();
                            temp.Id = schedule.Id;
                            temp.UserId = schedule.UserId;
                            temp.Name = schedule.Name;
                            temp.Url = schedule.Url;
                            resultSchedules.Add(temp);
                        }
                        return resultSchedules;
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("Fehler beim lokalen Abrufen der Trainingspl�ne: " + exc.StackTrace);
                    return null;
                }
            }
        }
        #endregion

        #region Exercise
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <returns></returns>
        public async Task<ExerciseModel> GetExerciseByIdAsync(int exerciseId)
        {
            if (Online)
            {
                try
                {
                    ExerciseModel exercise = await mgnService.GetExerciseByIdAsync(exerciseId);
                    Exercise insert = new Exercise();
                    insert.Id = exercise.Id;
                    insert.Name = exercise.Name;
                    insert.Url = exercise.Url;
                    insert.Description = exercise.Description;
                    db.insertUpdateExercise(insert);
                    return exercise;
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Abrufen einer �bung: " + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                try
                {
                    Exercise exercise = db.GetExerciseById(exerciseId);
                    ExerciseModel output = new ExerciseModel();
                    output.Id = exercise.Id;
                    output.Name = exercise.Name;
                    output.Url = exercise.Url;
                    output.Description = exercise.Description;
                    List<EntryModel<int>> temp = new List<EntryModel<int>>();
                    foreach (var item in db.GetAllSchedulesByExercise(output.Id))
                    {
                        EntryModel<int> e = new EntryModel<int>();
                        e.Id = item;
                        temp.Add(e);
                    }
                    output.Schedules = temp;
                    return output;
                }
                catch(Exception exc)
                {
                    System.Console.WriteLine("Fehler beim lokalen Abrufen einer �bung: " + exc.StackTrace);
                    return null;
                }
            }
        }

        /// <summary>
        /// Gibt einen Trainingsplan zur�ck
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ScheduleModel> GetScheduleByIdAsync(int id)
        {
            if (Online)
            {
                try
                {
                    ScheduleModel schedule = await mgnService.GetScheduleByIdAsync(id);
                    return schedule;
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Online-Abrufen eines Trainingsplans: " + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                try
                {
                    Schedule schedule = db.GetScheduleById(id);
                    ScheduleModel result = new ScheduleModel();
                    result.Id = schedule.Id;
                    result.Name = schedule.Name;
                    result.Url = schedule.Url;
                    result.UserId = schedule.UserId;
                    List<EntryModel<int>> temp = new List<EntryModel<int>>();
                    foreach (var item in db.GetAllExercisesBySchedule(result.Id))
                    {
                        EntryModel<int> e = new EntryModel<int>();
                        e.Id = item;
                        temp.Add(e);
                    }
                    result.Exercises = temp;
                    return result;
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("Fehler beim lokalen Abrufen eines Trainingsplans: " + exc.StackTrace);
                    return null;
                }
            }
        }

        /// <summary>
        /// Gibt die �bungen eines Trainingsplans zur�ck
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        public async Task<List<ExerciseModel>> GetExercisesForSchedule(int scheduleId)
        {
            if (Online)
            {
                try
                {
                    ScheduleModel schedule = await mgnService.GetScheduleByIdAsync(scheduleId);
                    IEnumerable<EntryModel<int>> scheduleExercises = schedule.Exercises;
                    List<ExerciseModel> result = new List<ExerciseModel>();
                    foreach (var exercise in scheduleExercises)
                    {
                        //F�r jede zugewiesene Id die Exercise raussuchen und der IEnumerable hinzuf�gen
                        result.Add(await mgnService.GetExerciseByIdAsync(exercise.Id));
                    }
                    return result;
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Online-Abrufen der �bungen eines Trainingsplans: " + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                try
                {
                    //Anhand der scheduleId alle Exercises aus der ScheduleHasExercises-Tabelle holen
                    IEnumerable<ScheduleHasExercises> scheduleHasExercises = db.GetExercisesOfSchedule(scheduleId);
                    if(scheduleHasExercises != null)
                    {
                        List<ExerciseModel> result = new List<ExerciseModel>();
                        foreach (var sHE in scheduleHasExercises)
                        {
                            Exercise exercise = db.GetExerciseById(sHE.ExerciseId);
                            ExerciseModel temp = new ExerciseModel();
                            temp.Id = exercise.Id;
                            temp.Name = exercise.Name;
                            temp.Url = exercise.Url;
                            temp.Description = exercise.Description;
                            result.Add(temp);
                        }
                        return result;
                    }
                    Console.WriteLine("Keine �bungen zu dem Trainingsplan vorhanden");
                    return null;
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("Fehler beim lokalen Abrufen eines Trainingsplans: " + exc.StackTrace);
                    return null;
                }
            }
        }
        #endregion


        //public StartCommandResult OnStartCommand -> Handling des Neustarts des Services
        //Sticky -> startet bei null
        //RedeliverIntent -> macht da weiter wo er aufgeh�rt hat
        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Console.WriteLine("OnOffService gestartet!");
            Task.Run(async () =>
            {
                //Verbindungs�berpr�fung
                while (true)
                {
                    //lock (thisLock)
                    //{
                        //var result = mgnServiceServer.PingAsync();
                        //if (result.Result)
                        if(await mgnServiceServer.PingAsync())
                        {
                            //Online = true;
                            setzeStatus(true);
                        }
                        else
                        {
                            //Online = false;
                            setzeStatus(true);
                        }

                    Console.WriteLine("Online: " + Online);
                    //Timeout 10sek.
                    System.Threading.Thread.Sleep(10000);
                    //}
                    //Haken entsprechend der Connection setzen
                    //mainA.setConnectivityStatus(online);
                    //exerciseA.setConnectivityStatus(online);
                    //practiceA.setConnectivityStatus(online);
                    //scheduleA.setConnectivityStatus(online);
                }
            });


            return StartCommandResult.Sticky;
        }

        //OnDestroy -> Nach Beendigung
        public override void OnDestroy()
        {
            base.OnDestroy();
            // cleanup code
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
}