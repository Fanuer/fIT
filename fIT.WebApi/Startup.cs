using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using fIT.WebApi.Manager;
using fIT.WebApi.Repository;
using Newtonsoft.Json.Serialization;
using Owin;

namespace fIT.WebApi
{
    public class Startup
    {
        #region Methods
        /// <summary>
        /// Get's fired when the applications is started by the host
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();
            ConfigureOAuthTokenGeneration(app);
            ConfigureWebApi(httpConfig);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(httpConfig);
        }

        /// <summary>
        /// Configure the db context and user manager to use a single instance per request
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Plugin the OAuth bearer JSON Web Token tokens generation and Consumption will be here

        }

        /// <summary>
        /// Defines Web Api Routing and Json-Response-Resolver 
        /// </summary>
        /// <param name="config"></param>
        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        #endregion
    }
}
