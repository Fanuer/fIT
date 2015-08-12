using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using fIT.WebApi.Client.Data.Models.Shared.Enums;
using System.Threading.Tasks;
using fITNat.Services;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fIT.WebApi.Client.Data.Models.Schedule;
using fIT.WebApi.Client.Data.Models.Exercise;
using fITNat.DBModels;
using fIT.WebApi.Client.Portable.Implementation;
using fIT.WebApi.Client.Data.Models.Shared;

namespace fITNat
{
    [Service]
    public class OnOffService : Service
    {
        #region Variablen
        private ManagementServiceLocal mgnService;
        private ManagementService mgnServiceServer;
        private const string URL = @"http://fit-bachelor.azurewebsites.net/";
        private LocalDB db;
        private Guid userID;
        public static bool Online { get; private set; }
        private Object thisLock = new Object();
        private IBinder mBinder;
        #endregion


        public static void setzeStatus(bool data)
        {
            Online = data;
        }

        //OnCreate -> Initialisierung
        public override void OnCreate()
        {
            base.OnCreate(); 
            mgnServiceServer = new ManagementService(URL);
            mgnService = new ManagementServiceLocal();
            db = new LocalDB();
        }

        #region User
        /// <summary>
        /// Entsprechend des Netzwerkstatus wird lokal oder am Server angelegt
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> SignIn(string username, string password)
        {
            User user = new User();
            db = new LocalDB();
            user.Username = username;
            user.Password = password;
            userID = user.UserId;
            mgnService = new ManagementServiceLocal();
            Console.WriteLine("Online vor der Entscheidung: " + Online);
            if (Online)
            {
                try
                {
                    await mgnService.SignIn(username, password);
                    Console.WriteLine("läuft!");
                    //Benutzer abfragen, die auf dem Server liegen und mit Lokal abgleichen
                    await db.insertUpdateUser(user);
                    System.Console.WriteLine("Online eingeloggt");
                    return true;
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
                    var result = await db.findUser(username, password);
                    if (result.Count != 1)
                    {
                        return false;
                    }
                    System.Console.WriteLine("Offline eingeloggt");
                }
                catch(Exception exc)
                {
                    System.Console.WriteLine("Fehler beim Offline-Einloggen" + exc.StackTrace);
                }
            }
            return false;
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
            mgnService = new ManagementServiceLocal();
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
            mgnService = new ManagementServiceLocal();
            if (Online)
            {
                try
                {
                    if (await mgnService.recordPractice(scheduleId, exerciseId, timestamp, weight, repetitions, numberOfRepetitions))
                        return true;
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
                    var result = db.insertPractice(userId, scheduleId, exerciseId, timestamp, weight, repetitions, numberOfRepetitions);
                    int rueckgabeWert = result.Result;
                    if (rueckgabeWert == 1)
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
        /// Gibt alle Trainingspläne eines Benutzers zurück
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ScheduleModel>> GetAllSchedulesAsync(Guid userID = new Guid())
        {
            mgnService = new ManagementServiceLocal();
            if (Online)
            {
                try
                {
                    IEnumerable<ScheduleModel> schedules = await mgnService.GetAllSchedulesAsync();
                    return schedules;
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Abrufen der Trainingspläne: " + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                try
                {
                    //Alle Trainingspläne für den User zurückgeben
                    IEnumerable<Schedule> schedules = await db.GetAllSchedulesAsync(userID);
                    List<ScheduleModel> resultSchedules = new List<ScheduleModel>();
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
                catch (Exception exc)
                {
                    System.Console.WriteLine("Fehler beim lokalen Abrufen der Trainingspläne: " + exc.StackTrace);
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
            mgnService = new ManagementServiceLocal();
            if (Online)
            {
                try
                {
                    ExerciseModel exercise = await mgnService.GetExerciseByIdAsync(exerciseId);
                    return exercise;
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Abrufen einer Übung: " + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                try
                {
                    return null;
                }
                catch(Exception exc)
                {
                    System.Console.WriteLine("Fehler beim lokalen Abrufen einer Übung: " + exc.StackTrace);
                    return null;
                }
            }
        }

        /// <summary>
        /// Gibt einen Trainingsplan zurück
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ScheduleModel> GetScheduleByIdAsync(int id)
        {
            mgnService = new ManagementServiceLocal();
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
                    Schedule schedule = await db.GetScheduleByIdAsync(id);
                    ScheduleModel result = new ScheduleModel();
                    result.Id = schedule.Id;
                    result.Name = schedule.Name;
                    result.Url = schedule.Url;
                    result.UserId = schedule.UserId;
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
        /// Gibt die Übungen eines Trainingsplans zurück
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        public async Task<List<ExerciseModel>> GetExercisesForSchedule(int scheduleId)
        {
            mgnService = new ManagementServiceLocal();
            if (Online)
            {
                try
                {
                    ScheduleModel schedule = await mgnService.GetScheduleByIdAsync(scheduleId);
                    IEnumerable<EntryModel<int>> scheduleExercises = schedule.Exercises;
                    List<ExerciseModel> result = new List<ExerciseModel>();
                    foreach (var exercise in scheduleExercises)
                    {
                        //Für jede zugewiesene Id die Exercise raussuchen und der IEnumerable hinzufügen
                        result.Add(await mgnService.GetExerciseByIdAsync(exercise.Id));
                    }
                    return result;
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Online-Abrufen der Übungen eines Trainingsplans: " + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                try
                {
                    //Anhand der scheduleId alle Exercises aus der ScheduleHasExercises-Tabelle holen
                    IEnumerable<ScheduleHasExercises> scheduleHasExercises = await db.GetExercisesOfSchedule(scheduleId);
                    List<ExerciseModel> result = new List<ExerciseModel>();
                    foreach (var sHE in scheduleHasExercises)
                    {
                        Exercise exercise = await db.GetExerciseByIdAsync(sHE.ExerciseId);
                        ExerciseModel temp = new ExerciseModel();
                        temp.Id = exercise.Id;
                        temp.Name = exercise.Name;
                        temp.Url = exercise.Url;
                        temp.Description = exercise.Description;
                        result.Add(temp);
                    }
                    return result;
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
        //RedeliverIntent -> macht da weiter wo er aufgehört hat
        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Console.WriteLine("OnOffService gestartet!");
            mgnService = new ManagementServiceLocal();
            Task.Run(async () =>
            {
                //Datenbank erstellen
                db = new LocalDB();

                //Erstellen der lokalen DB macht Probleme!!!
                var task = db.createDatabase();
                if (task.Result)
                {
                    //Verbindungsüberprüfung
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