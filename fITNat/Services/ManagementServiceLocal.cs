using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using fIT.WebApi.Client.Portable.Implementation;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Shared.Enums;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Practice;
using fIT.WebApi.Client.Data.Models.Schedule;
using System.Collections.Generic;
using fIT.WebApi.Client.Data.Models.Exercise;
using System.Threading;
using fIT.WebApi.Client.Data.Models.Shared;

namespace fITNat.Services
{
    [Service]
    public class ManagementServiceLocal : Service
    {
        #region Variablen
        private const string URL = @"http://fit-bachelor.azurewebsites.net/";
        public static ManagementService service { get; private set; }
        private string folder;
        private string username { get; set; }
        private string password { get; set; }
        private IManagementSession session;
        private IBinder mBinder;
        #endregion

        public async static Task createTestdata(IManagementSession session)
        {
            
            Guid userId = session.CurrentUserId;
            ScheduleModel s = new ScheduleModel();
            s.Name = "Arbeitstraining";
            s.UserId = userId.ToString();
            ScheduleModel back = await session.Users.CreateScheduleAsync(s);
            Console.WriteLine("Schedule angelegt");
            var e1 = new ExerciseModel()
            {
                Name = "Bankdrücken",
                Description = "Stange von der Brust wegdrücken"
            };
            var e2 = new ExerciseModel()
            {
                Name = "Kniebeuge",
                Description = "Ordentlich aus den Oberschnkeln drücken"
            };
            var e3 = new ExerciseModel()
            {
                Name = "Beinpresse",
                Description = "Schweres Gewicht"
            };
            ExerciseModel temp1 = await session.Admins.CreateExerciseAsync(e1);
            ExerciseModel temp2 = await session.Admins.CreateExerciseAsync(e2);
            ExerciseModel temp3 = await session.Admins.CreateExerciseAsync(e3);
            await session.Users.AddExerciseToScheduleAsync(back.Id, temp1.Id);
            await session.Users.AddExerciseToScheduleAsync(back.Id, temp2.Id);
            await session.Users.AddExerciseToScheduleAsync(back.Id, temp3.Id);
            Console.WriteLine("Übungen hinzugefügt");


            /*EntryModel<int> a1 = new EntryModel<int>();
            a1.Id = temp1.Id;
            exercises.Add(a1);
            ExerciseModel temp2 = await session.Admins.CreateExerciseAsync(e2);
            EntryModel<int> a2 = new EntryModel<int>();
            a2.Id = temp2.Id;
            exercises.Add(a2);
            ExerciseModel temp3 = await session.Admins.CreateExerciseAsync(e3);
            EntryModel<int> a3 = new EntryModel<int>();
            a3.Id = temp3.Id;
            exercises.Add(a3);
            ScheduleModel sUpdate = back;
            sUpdate.Exercises = exercises;
            var t = session.Users.UpdateScheduleAsync(sUpdate.Id, sUpdate);
            Console.WriteLine("Exercises angelegt");
            /*PracticeModel p = new PracticeModel();
            p.ExerciseId = temp1.Id;
            p.ScheduleId = back.Id;
            p.NumberOfRepetitions = 4;
            p.Repetitions = 12;
            p.Timestamp = DateTime.Now;
            p.UserId = userId.ToString();
            p.Weight = 120.9;
            PracticeModel tempP = await session.Users.CreatePracticeAsync(p);
            PracticeModel p2 = new PracticeModel();
            p2.ExerciseId = temp2.Id;
            p2.ScheduleId = back.Id;
            p2.NumberOfRepetitions = 5;
            p2.Repetitions = 15;
            p2.Timestamp = DateTime.Now;
            p2.UserId = userId.ToString();
            p2.Weight = 40;
            PracticeModel tempP2 = await session.Users.CreatePracticeAsync(p2);
            Console.WriteLine("Practices angelegt");*/
        }

        public static void setzeManagementService(ManagementService data)
        {
            service = data;
        }

        public override IBinder OnBind(Intent intent)
        {
            return mBinder;
        }

        //OnCreate -> Initialisierung
        public override void OnCreate()
        {
            base.OnCreate();
            try
            {
                setzeManagementService(new ManagementService(URL));
                Console.WriteLine("ManagementServiceLocal gestartet!");
            }
            catch (ServerException e)
            {
                Console.WriteLine("Serverfehler: " + e.StackTrace);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Fehler bei OnCreate in ManagementServiceLocal");
            }
        }

        /// <summary>
        /// Gibt die aktuelle Session zurück
        /// </summary>
        /// <returns>aktuelle Session</returns>
        public IManagementSession actualSession()
        {
            return session;
        }

        #region User
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> SignIn(string username, string password)
        {
            bool result = false;
            this.username = username;
            this.password = password;
            try
            {
                //Muss auf irgendwas warten => im Debug geht es dann!
                var v = await Task.WhenAny(service.LoginAsync(username, password));
                session = v.Result; //Exception bei falschem Login
                //await createTestdata(session);
                result = true;
            }
            catch (ServerException e)
            {
                Console.WriteLine("Serverfehler: " + e.StackTrace);
                throw;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Login-Fehler: " + exc.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Registrierung am Server
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
        public async Task SignUp(
                                string username,
                                string email,
                                string password,
                                string passwordConfirm,
                                GenderType gender,
                                JobTypes job,
                                FitnessType fitness,
                                DateTime birthdate)
        {
            try
            {
                CreateUserModel user = new CreateUserModel();
                user.Username = username;
                user.Password = password;
                user.ConfirmPassword = passwordConfirm;
                user.Gender = gender;
                user.Job = job;
                user.Fitness = fitness;
                user.DateOfBirth = birthdate;
                await service.RegisterAsync(user);
            }
            catch (ServerException ex)
            {
                throw;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Fehler beim Registrieren: " + exc.StackTrace);
            }
        }
        #endregion

        #region Schedule
        /// <summary>
        /// Ruft alle Trainingspläne vom Server ab
        /// </summary>
        /// <returns>IEnumerable aller Pläne</returns>
        public async Task<IEnumerable<ScheduleModel>> GetAllSchedulesAsync()
        {
            try
            {
                IEnumerable<ScheduleModel> schedules = await session.Users.GetAllSchedulesAsync();
                return schedules;
            }
            catch (ServerException ex)
            {
                throw;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Fehler beim Online-Abrufen der Trainingspläne: " + exc.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Ruft einen Trainingsplan an Hand der ID vom Server ab
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ScheduleModel> GetScheduleByIdAsync(int id)
        {
            try
            {
                ScheduleModel schedule = await session.Users.GetScheduleByIdAsync(id);
                return schedule;
            }
            catch (ServerException ex)
            {
                throw;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Fehler beim Online-Abrufen eines Trainingsplans: " + exc.StackTrace);
                return null;
            }
        }
        #endregion

        #region Exercise
        /// <summary>
        /// Ruft eine Übung anhand der ID vom Server ab
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <returns></returns>
        public async Task<ExerciseModel> GetExerciseByIdAsync(int exerciseId)
        {
            try
            {
                ExerciseModel exercise = await session.Users.GetExerciseByIdAsync(exerciseId);
                return exercise;
            }
            catch (ServerException ex)
            {
                throw;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Fehler beim Online-Abrufen einer Übung: " + exc.StackTrace);
                return null;
            }
        }

        public async Task<IEnumerable<ExerciseModel>> GetAllExercisesAsync()
        {
            try
            {
                IEnumerable<ExerciseModel> exercises = await session.Users.GetAllExercisesAsync();
                return exercises;
            }
            catch (ServerException ex)
            {
                throw;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Fehler beim Online-Abrufen einer Übung: " + exc.StackTrace);
                return null;
            }
        }
        #endregion

        #region Practice
        /// <summary>
        /// Legt eine Übung am Server an
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="exerciseId"></param>
        /// <param name="timestamp"></param>
        /// <param name="weight"></param>
        /// <param name="repetitions"></param>
        /// <param name="numberOfRepetitions"></param>
        /// <returns></returns>
        public async Task<bool> recordPractice(int scheduleId,
                                        int exerciseId,
                                        DateTime timestamp = default(DateTime),
                                        double weight = 0,
                                        int repetitions = 0,
                                        int numberOfRepetitions = 0)
        {
            try
            {
                PracticeModel practice = new PracticeModel();
                practice.ScheduleId = scheduleId;
                practice.ExerciseId = exerciseId;
                practice.Timestamp = timestamp;
                practice.Weight = weight;
                practice.Repetitions = repetitions;
                practice.NumberOfRepetitions = numberOfRepetitions;
                PracticeModel result = await session.Users.CreatePracticeAsync(practice);
                if (result != null)
                    return true;
                else
                    return false;
            }
            catch (ServerException ex)
            {
                throw;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Fehler beim Eintragen eines Trainings: " + exc.StackTrace);
            }
            return false;
        }
        #endregion


        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }
    }
}