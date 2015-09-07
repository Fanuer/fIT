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
using fIT.WebApi.Client.Data.Models.Practice;

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
        public static bool WasOffline { get; private set; }
        private Object thisLock = new Object();
        private IBinder mBinder;
        #endregion

        #region statische Variablen belegen
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

        public static void setzeWasOffline(bool data)
        {
            WasOffline = data;
        }
        #endregion

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
            //Console.WriteLine("Online vor der Entscheidung: " + Online);
            if (Online)
            {
                try
                {
                    bool success = await mgnService.SignIn(username, password);
                    if (success)
                    {
                        userId = mgnService.actualSession().CurrentUserId;
                        user.UserId = userId.ToString();
                        if (db != null)
                        {
                            db.insertUpdateUser(user);
                            Console.WriteLine("User eingeloggt und lokal abgelegt");
                        }
                        return userId;
                    }
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Online-Einloggen (Server)" + ex.StackTrace);
                    throw;
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("Fehler beim Online-Einloggen" + exc.StackTrace);
                }
                return new Guid();
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
                catch (Exception exc)
                {
                    System.Console.WriteLine("Fehler beim Offline-Einloggen" + exc.StackTrace);
                    throw;
                }
                return new Guid();
            }
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
        public async Task<bool> SignUp(string username,
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
                    User user = new User();
                    user.UserId = await mgnService.SignUp(username, email, password, passwordConfirm, gender, job, fitness, birthdate);
                    user.Username = username;
                    user.Password = password;
                    user.wasOffline = false;
                    db.insertUpdateUser(user);
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
        public async Task<bool> createPracticeAsync(int scheduleId,
                                                int exerciseId,
                                                string userId,
                                                DateTime timestamp = default(DateTime),
                                                double weight = 0,
                                                int repetitions = 0,
                                                int numberOfRepetitions = 0)
        {
            Practice pOff;
            Practice pOn;
            int id;
            if (Online)
            {
                try
                {
                    id = await mgnService.recordPractice(scheduleId, exerciseId, userId, timestamp, weight, repetitions, numberOfRepetitions);
                    if (id != 0)
                    {
                        pOn = new Practice();
                        pOn.UserId = userId;
                        pOn.Id = id;
                        pOn.ScheduleId = scheduleId;
                        pOn.ExerciseId = exerciseId;
                        pOn.Timestamp = timestamp;
                        pOn.Weight = weight;
                        pOn.Repetitions = repetitions;
                        pOn.NumberOfRepetitions = numberOfRepetitions;
                        int result = db.insertUpdatePracticeOnline(pOn);
                        if (result != -1)
                        {
                            Console.WriteLine("Training auch lokal angelegt");
                        }
                        return true;
                    }
                    else
                        return false;
                }
                catch (ServerException ex)
                {
                    throw;
                }
            }
            else
            {
                try
                {
                    pOff = new Practice();
                    pOff.UserId = userId;
                    pOff.ScheduleId = scheduleId;
                    pOff.ExerciseId = exerciseId;
                    pOff.Timestamp = timestamp;
                    pOff.Weight = weight;
                    pOff.Repetitions = repetitions;
                    pOff.NumberOfRepetitions = numberOfRepetitions;
                    pOff.WasOffline = true;
                    int result = db.insertUpdatePracticeOffline(pOff);
                    if (result != -1)
                        return true;
                    else
                        return false;
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("Fehler beim lokalen Anlegen des Trainings: " + exc.StackTrace);
                }
            }
            return true;
        }

        public async Task<List<Practice>> getAllPracticesAsync(string userId, int scheduleId, int exerciseId)
        {
            List<Practice> resultListe = new List<Practice>();
            Practice pOn;
            if (Online)
            {
                try
                {
                    List<PracticeModel> practices;
                    practices = await mgnService.getAllPracticesAsync(scheduleId, exerciseId);
                    foreach (var item in practices)
                    {
                        if (item.Id != 0)
                        {
                            pOn = new Practice();
                            pOn.UserId = item.UserId;
                            pOn.Id = item.Id;
                            pOn.ScheduleId = item.ScheduleId;
                            pOn.ExerciseId = item.ExerciseId;
                            pOn.Timestamp = item.Timestamp;
                            pOn.Weight = item.Weight;
                            pOn.Repetitions = item.Repetitions;
                            pOn.NumberOfRepetitions = item.NumberOfRepetitions;
                            resultListe.Add(pOn);
                            int result = db.insertUpdatePracticeOnline(pOn);
                            if (result != -1)
                            {
                                Console.WriteLine("Training auch lokal angelegt");
                            }
                        }
                    }
                    return resultListe;
                }
                catch (ServerException ex)
                {
                    System.Console.WriteLine("Fehler beim Holen aller Trainings: " + ex.StackTrace);
                    throw;
                }
            }
            else
            {
                try
                {
                    List<Practice> practices;
                    practices = db.GetAllPracticesByUserScheduleExercise(userId, scheduleId, exerciseId);
                    foreach (var item in practices)
                    {
                        if (item.Id != 0 || item.LocalId != 0)
                        {
                            resultListe.Add(item);
                        }
                    }
                    return resultListe;
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("Fehler beim offline Abrufen der Trainings: " + exc.StackTrace);
                }
            }
            return resultListe;
        }
        #endregion

        #region Schedule
        /// <summary>
        /// Gibt alle Trainingspläne eines Benutzers zurück
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ScheduleModel>> GetAllSchedulesAsync(Guid userID = new Guid())
        {
            if (Online)
            {
                try
                {
                    IEnumerable<ScheduleModel> schedules = await mgnService.GetAllSchedulesAsync().ConfigureAwait(continueOnCapturedContext: false);
                    if(schedules != null)
                    {
                        foreach (var item in schedules)
                        {
                            Schedule s = new Schedule();
                            s.Id = item.Id;
                            s.Name = item.Name;
                            s.UserId = item.UserId;
                            s.Url = item.Url;
                            s.WasOffline = false;
                            db.insertUpdateSchedule(s);
                            foreach (var data in item.Exercises)
                            {
                                bool a = db.InsertScheduleHasExercises(item.Id, data.Id, false);
                            }
                        }
                        return schedules;
                    }
                    return null;
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
                    System.Console.WriteLine("Fehler beim Abrufen einer Übung: " + ex.StackTrace);
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
                    System.Console.WriteLine("Fehler beim lokalen Abrufen einer Übung: " + exc.StackTrace);
                    return null;
                }
            }
        }
        /*
        /// <summary>
        /// Gibt einen Trainingsplan zurück
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
        }*/

        /// <summary>
        /// Gibt die Übungen eines Trainingsplans zurück
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
                        //Für jede zugewiesene Id die Exercise raussuchen und der IEnumerable hinzufügen
                        ExerciseModel temp = await mgnService.GetExerciseByIdAsync(exercise.Id);
                        result.Add(temp);
                    }
                    //Übernehmen der Exercises in die lokale DB
                    foreach (var item in result)
                    {
                        Exercise e = new Exercise();
                        e.Description = item.Description;
                        e.Id = item.Id;
                        e.Name = item.Name;
                        e.Url = item.Url;
                        e.WasOffline = false;
                        db.insertUpdateExercise(e);
                        //db.InsertScheduleHasExercises(scheduleId, e.Id, false);
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
                    List<ExerciseModel> result = new List<ExerciseModel>();
                    //Anhand der scheduleId alle Exercises aus der ScheduleHasExercises-Tabelle holen
                    Schedule test = db.GetScheduleById(scheduleId);
                    List<Exercise> exercises = db.GetExercisesOfSchedule(scheduleId);
                    foreach (var item in exercises)
                    {
                        ExerciseModel e = new ExerciseModel();
                        e.Description = item.Description;
                        e.Name = item.Name;
                        e.Url = item.Name;
                        e.Id = item.Id;
                        List<EntryModel<int>> schedules = new List<EntryModel<int>>();
                        EntryModel<int> temp = new EntryModel<int>();
                        temp.Id = scheduleId;
                        schedules.Add(temp);
                        e.Schedules = schedules;
                        result.Add(e);
                    }
                    if(result.Count == 0)
                    {
                        Console.WriteLine("Keine Übungen zu dem Trainingsplan vorhanden");
                        return null;
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
            Task.Run(async () =>
            {
                //Verbindungsüberprüfung
                while (true)
                {
                    bool status = false;
                    try
                    {
                        status = await mgnServiceServer.PingAsync();
                    }
                    catch(Exception ex)
                    {
                        status = false;
                    }
                    finally
                    {
                        if (status)
                        {
                            //Online = true;
                            setzeStatus(true);
                            //vorher Offline => jetzt die Aktionen ausführen, die nur lokal gemacht werden konnten
                            if (WasOffline)
                            {
                                await checkSync();
                                setzeWasOffline(false);
                            }
                        }
                        else
                        {
                            //Online = false;
                            setzeStatus(false);
                            setzeWasOffline(true);
                        }
                        Console.WriteLine("Online: " + Online);                            
                        //Timeout 10sek.
                        System.Threading.Thread.Sleep(10000);
                    }
                }
            });
            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// Überprüft die lokale DB nach Änderungen, die zum Server hochgeladen werden müssen (besitzen den lokalen Stempel)
        /// Über die Daten wird iteriert, um minimal viele nur zu verlieren
        /// </summary>
        private async Task checkSync()
        {
            List<Practice> offPractices = db.GetOfflinePractice();
            int result;
            if(offPractices.Count != 0)
            {
                foreach (var item in offPractices)
                {
                    try
                    {
                        User u = db.findUser(item.UserId);
                        result = await mgnService.recordPractice(item.ScheduleId, item.ExerciseId, item.UserId, item.Timestamp, item.Weight, item.Repetitions, item.NumberOfRepetitions, u.Username, u.Password);
                        if (result != 0)
                        {
                            item.Id = result;
                            if (db.setPracticeOnline(item))
                                Console.WriteLine("Offline angelegtes Training " + item.Id + " ist hochgeladen");
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Fehler im Sync: " + ex.StackTrace);
                        break;
                    }
                }
            }
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