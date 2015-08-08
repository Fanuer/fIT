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
using fIT.WebApi.Client.Data.Models.Account;
using fITNat.DBModels;
using fIT.WebApi.Client.Portable.Implementation;

namespace fITNat
{
    [Service]
    class OnOffService : Service
    {
        #region Variablen
        private ConnectivityService conService;
        private ManagementServiceLocal mgnService;
        private ManagementService mgnServiceServer;
        private const string URL = @"http://fit-bachelor.azurewebsites.net/";
        private bool online;
        private LocalDB db;
        private string userID;
        #endregion

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        //OnCreate -> Initialisierung
        public async override void OnCreate()
        {
            base.OnCreate();
            conService = new ConnectivityService();
            mgnService = new ManagementServiceLocal();
            mgnServiceServer = new ManagementService(URL);
            online = await mgnServiceServer.PingAsync();
            db = new LocalDB();
            userID = "";
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
            userID = user.Username;
            if (online)
            {
                try
                {
                    await mgnService.SignIn(username, password);
                    //Benutzer abfragen, die auf dem Server liegen und mit Lokal abgleichen
                    await db.insertUpdateUser(user);
                    System.Console.WriteLine("Online eingeloggt");
                    return true;
                }
                catch(ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Online-Einloggen" + ex.StackTrace);
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
        /// Entsprechend des Netzwerkstatus wird lokal oder am Server angelegt
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
            if (online)
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
            if (online)
            {
                try
                {
                    string user = mgnService.actualSession().CurrentUserName;
                    await mgnService.recordPractice(scheduleId, exerciseId, timestamp, weight, repetitions, numberOfRepetitions);
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
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ScheduleModel>> GetAllSchedulesAsync()
        {
            if (online)
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
            if (online)
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
            if (online)
            {
                try
                {
                    ScheduleModel schedule = await mgnService.GetScheduleByIdAsync(id);
                    return schedule;
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Abrufen eines Trainingsplans: " + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                try
                {
                    //TODO
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
        //RedeliverIntent -> macht da weiter wo er aufgehört hat
        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Console.WriteLine("OnOffService gestartet!");
            Task.Run(async () =>
            {
                MainActivity mainA = new MainActivity();
                ExerciseActivity exerciseA = new ExerciseActivity();
                PracticeActivity practiceA = new PracticeActivity();
                ScheduleActivity scheduleA = new ScheduleActivity();

                Console.WriteLine("Datenbank erstellen");
                //Datenbank erstellen
                db = new LocalDB();

                //Erstellen der lokalen DB macht Probleme!!!
                var task = db.createDatabase();
                if (task.Result)
                {
                    //Verbindungsüberprüfung
                    Console.WriteLine("Vor der While-Schleife");
                    while (true)
                    {
                        if (await mgnServiceServer.PingAsync())
                        {
                            online = true;
                        }
                        else
                        {
                            online = false;
                        }
                        //Timeout 10sek.
                        System.Threading.Thread.Sleep(10000);

                        //Haken entsprechend der Connection setzen
                        mainA.setConnectivityStatus(online);
                        exerciseA.setConnectivityStatus(online);
                        practiceA.setConnectivityStatus(online);
                        scheduleA.setConnectivityStatus(online);
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
    }
}