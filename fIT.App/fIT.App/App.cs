using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using AutoMapper;
using fIT.App.Data.Datamodels;
using fIT.App.Data.ViewModels;
using fIT.App.Helpers;
using fIT.App.Helpers.Navigation;
using fIT.App.Helpers.Navigation.Interfaces;
using fIT.App.Helpers.Navigation.Specific;
using fIT.App.Interfaces;
using fIT.App.Pages;
using fIT.App.Repositories;
using fIT.App.Services;
using fIT.WebApi.Client.Data.Models.Exercise;
using fIT.WebApi.Client.Data.Models.Schedule;
using Xamarin.Forms;

namespace fIT.App
{
    public class App : Application
    {

        #region FIELDS
        #endregion

        #region CTOR
        public App()
        {
                   AppName = "Fitter";
            IoCLocator.Current.RegisterServices(new Dictionary<Type, Type>()
            {
                { typeof(IRepository), typeof(Repository) },
            });
            //IoCLocator.Current.RegisterViewModels(typeof(AppViewModelBase).Namespace);
            IoCLocator.Current.RegisterService(() => UserDialogs.Instance);
            RegisterAutoMapper();

            NavigationFrame frame = null;

            // Is Logged In
            if (String.IsNullOrEmpty(Settings.RefreshToken))
            {
                //var vm = IoCLocator.Current.GetInstance<LoginViewModel>();
                var vm = new LoginViewModel();
                frame = new NavigationFrame(vm);
            }
            else
            {
                //var vm = IoCLocator.Current.GetInstance<ScheduleViewModel>();
                var vm = new ScheduleViewModel();
                frame = new NavigationFrame(vm);
            }

            MainPage = frame.Root;
        }

        #endregion

        #region METHODS
        protected override void OnStart()
        {
            OnOffService.Current.Start();
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            OnOffService.Current.Stop();
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            OnOffService.Current.Start();
            // Handle when your app resumes
        }

        private void RegisterAutoMapper()
        {
            IoCLocator.Current.RegisterService(() =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ScheduleModel, ScheduleListEntryViewModel>()
                    .ForMember(vm => vm.ExerciseCount, mapConfig => mapConfig.ResolveUsing(m => m.Exercises.Count));

                    cfg.CreateMap<ScheduleListEntryViewModel, ExerciseViewModel>()
                        .ConstructUsing(model => new ExerciseViewModel(model.Name))
                        .ForMember(exercise=> exercise.Title, mapConfig => mapConfig.ResolveUsing(m => m.Name));

                    cfg.CreateMap<ExerciseModel, ExerciseListEntryViewModel>();
                });

                return config.CreateMapper();
            });
        }
        #endregion

        #region PROPERTIES
        public static string AppName { get; private set; }

        #endregion
    }
}
