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

namespace fITNat.Services
{
    [Service]
    class ManagementServiceLocal : Service
    {
        private const string URL = @"http://fit-bachelor.azurewebsites.net/";
        private ManagementService service;
        private string username { get; set; }
        private string password { get; set; }
        private IManagementSession session;

        public override void OnCreate()
        {
            try {
                //Testweise
                service = new ManagementService(URL);
                base.OnCreate();
            }
            catch(ServerException e)
            {
                Console.WriteLine("Serverfehler: " + e.StackTrace);
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public async Task SignIn(string username, string password)
        {
            this.username = username;
            this.password = password;
            try
            {
                session = await service.LoginAsync(username, password);
            }
            catch(ServerException e)
            {
                Console.WriteLine("Serverfehler: " + e.StackTrace);
                //Falsche Logindaten
                //Rückmeldung an den Login-Dialog, dass die Kombination User+PW nicht passt
                //e.Data => Alle Fehler!!
                Dialog_SignIn signInD = new Dialog_SignIn();
                signInD.SignInFail();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Login-Fehler: " + exc.StackTrace);
            }
        }

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
            catch(ServerException ex)
            {
                Console.WriteLine("Serverfehler: " + ex.StackTrace);
                //Falsche Logindaten
                //Rückmeldung an den Login-Dialog, dass die Kombination User+PW nicht passt
                Dialog_SignUp signUpD = new Dialog_SignUp();
                string fehler = "";
                if (ex.StackTrace.Contains("username"))
                    fehler = "username";
                else if (ex.StackTrace.Contains("username"))
                    fehler = "password";
                signUpD.SignUpFail(fehler);
            }
        }

        public async Task recordPractice(int id,
                                        int scheduleId,
                                        int exerciseId,
                                        string userId,
                                        DateTime timestamp = default(DateTime),
                                        double weight = 0,
                                        int repetitions = 0,
                                        int numberOfRepetitions = 0)
        {
            try
            {
                PracticeModel practice = new PracticeModel(id, scheduleId, exerciseId, userId, timestamp, weight, repetitions, numberOfRepetitions);
            }
            catch(ServerException ex)
            {
                throw;
            }
            catch(Exception exc)
            {
                Console.WriteLine("Fehler beim Eintragen eines Trainings: " + exc.StackTrace);
            }
        }
    }
}