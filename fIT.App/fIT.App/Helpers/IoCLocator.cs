using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace fIT.App.Helpers
{
    /// <summary>
    /// Inversion of Control handler for View Models and Services. 
    /// </summary>
    public class IoCLocator
    {
        #region FIELDS

        private static IoCLocator _current;
        #endregion

        #region CTOR
        /// <summary>
        /// Register all the used ViewModels, Services et. al. witht the IoC Container
        /// </summary>
        private IoCLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            this.DomainTypes = GetDomainAssemblies();
        }

        #endregion

        #region METHODS
        
        /// <summary>
        /// Registers all ViewModels that inherit from ViewModelBase and are located in the given namespace
        /// </summary>
        /// <param name="viewModelNameSpace"></param>
        public void RegisterViewModels(string viewModelNameSpace)
        {
            if (String.IsNullOrWhiteSpace(viewModelNameSpace))
            {
                return;
            }
            var viewModelTypes = this.DomainTypes.Where(x => !String.IsNullOrWhiteSpace(x?.Namespace) && x.Namespace.Equals(viewModelNameSpace) && x.IsSubclassOf(typeof(ViewModelBase)) && !x.IsAbstract);
            var myTypes = viewModelTypes.Select(x => x.FullName).ToList();
            //search for SimpleIoc.Default.Register<ViewModelBase>();

            var registerMethod = typeof(SimpleIoc).GetRuntimeMethods().First(x => x.IsPublic && x.Name.Equals("Register") && x.GetGenericArguments().Length == 1);

            foreach (var viewModelType in viewModelTypes)
            {
                var generic = registerMethod.MakeGenericMethod(viewModelType.AsType());
                generic.Invoke(SimpleIoc.Default, new object[] { });
            }
        }

        /// <summary>
        /// Registers all given Services 
        /// </summary>
        /// <param name="serviceTypes">Mapping of interface-Types an implementation-types for IoC</param>
        public void RegisterServices(IDictionary<Type, Type> serviceTypes)
        {
            if (serviceTypes == null)
            {
                throw new ArgumentNullException(nameof(serviceTypes));
            }

            //search for SimpleIoc.Default.Register<IService, Service>();
            var registerMethod = typeof(SimpleIoc).GetRuntimeMethods().First(x => x.IsPublic && x.Name.Equals("Register") && x.GetGenericArguments().Length == 2);
            foreach (var viewModelType in serviceTypes)
            {
                var generic = registerMethod.MakeGenericMethod(viewModelType.Key, viewModelType.Value);
                generic.Invoke(SimpleIoc.Default, new object[] {});
            }
        }

        /// <summary>
        /// Registers a single service
        /// </summary>
        /// <typeparam name="T">Type of the required class</typeparam>
        /// <param name="factory">Function to create the required object</param>
        public void RegisterService<T>(Func<T> factory) where T : class
        {
            if (!SimpleIoc.Default.IsRegistered<T>())
            {
                SimpleIoc.Default.Register<T>(factory);
            }
        }

        /// <summary>
        /// Returns an Instance of the given Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>() where T : class
        {
            T result = default(T);
            if (SimpleIoc.Default.IsRegistered<T>())
            {
                result = (T) ServiceLocator.Current.GetInstance(typeof(T));
            }
            return result;
        }

        /// <summary>
        /// Returns all typs of the current domain
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TypeInfo> GetDomainAssemblies()
        {
            // This is the hacky way we have to get the list of assemblies in a PCL for now.
            // Hopefully Xamarin will expose Device.GetAssemblies() in a future version of Xamarin.Forms.
            var currentDomain = typeof(string).GetTypeInfo().Assembly.GetType("System.AppDomain").GetRuntimeProperty("CurrentDomain").GetMethod.Invoke(null, new object[] { });
            var getAssemblies = currentDomain.GetType().GetRuntimeMethod("GetAssemblies", new Type[] { });
            var assemblies = getAssemblies.Invoke(currentDomain, new object[] { }) as Assembly[];
            return assemblies.SelectMany(a => a.DefinedTypes);

        }
        #endregion

        #region PROPERTIES

        /// <summary>
        /// All domain types 
        /// </summary>
        public IEnumerable<TypeInfo> DomainTypes { get; private set; }

        public static IoCLocator Current => IoCLocator._current ?? (IoCLocator._current = new IoCLocator());

        #endregion
    }
}
