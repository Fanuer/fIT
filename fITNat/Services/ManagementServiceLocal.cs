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
        /// Gibt die aktuelle Session zur�ck
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
                session = await service.LoginAsync(username, password);
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
        /// Ruft alle Trainingspl�ne vom Server ab
        /// </summary>
        /// <returns>IEnumerable aller Pl�ne</returns>
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
                Console.WriteLine("Fehler beim Online-Abrufen der Trainingspl�ne: " + exc.StackTrace);
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
        /// Ruft eine �bung anhand der ID vom Server ab
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
                Console.WriteLine("Fehler beim Online-Abrufen einer �bung: " + exc.StackTrace);
                return null;
            }
        }
        #endregion

        #region Practice
        /// <summary>
        /// Legt eine �bung am Server an
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