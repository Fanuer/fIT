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
using Android.Util;
using Java.IO;
using fIT.WebApi.Client.Data.Models.Shared.Enums;
using System.Threading.Tasks;
using fITNat.Services;
using fIT.WebApi.Client.Data.Models.Exceptions;

namespace fITNat
{
    [Service]
    class OnOffService : Service
    {
        private ConnectivityService conService;
        private ManagementServiceLocal mgnService;
        private bool online;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        //OnCreate -> Initialisierung
        public async override void OnCreate()
        {
            base.OnCreate();
            MainActivity mainA = new MainActivity();
            ExerciseActivity exerciseA = new ExerciseActivity();
            PracticeActivity practiceA = new PracticeActivity();
            ScheduleActivity scheduleA = new ScheduleActivity();

            while (true)
            {
                if(await conService.IsPingReachable())
                {
                    online = true;

                    

                    //Aufrufe nach online setzen und überprüfen, ob es Unterschiede gab
                }
                else
                {
                    online = false;
                    //Aufrufe nach offline setzen
                }
                //Haken entsprechend der Connection setzen
                mainA.setConnectivityStatus(online);
                exerciseA.setConnectivityStatus(online);
                practiceA.setConnectivityStatus(online);
                scheduleA.setConnectivityStatus(online);
            }
            

            //Datenhaltung
        }

        /// <summary>
        /// Entsprechend des Netzwerkstatus wird lokal oder am Server angelegt
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> SignIn(string username, string password)
        {
            if (online)
            {
                await mgnService.SignIn(username, password);
                //Benutzer abfragen, die auf dem Server liegen und mit Lokal abgleichen
            }
            else
            {
                //Lokal nachgucken
                //await localService.SignIn(username, password);
            }
            return true;
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
            if(online)
            {
                await mgnService.SignUp(username, email, password, passwordConfirm, gender, job, fitness, birthdate);
                //Lokal anlegen
            }
            else
            {
                //Fehler schmeissen, dass man offline ist!!!
                throw (new Exception("Registrierung funktioniert nur im Online-Modus"));
            }
            return true;
        }


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
        public async Task<bool> createPractice(int id,
                                    int scheduleId,
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
                    await mgnService.recordPractice(id, scheduleId, exerciseId, userId, timestamp, weight, repetitions, numberOfRepetitions);
                }
                catch(ServerException ex)
                {
                    throw;
                }
            }
            else
            {

            }
            return true;
        }



        //public StartCommandResult OnStartCommand -> Handling des Neustarts des Services
        //Sticky -> startet bei null
        //RedeliverIntent -> macht da weiter wo er aufgehört hat
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("DemoService", "DemoService started");
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