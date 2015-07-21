using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using fIT.WebApi.Models;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using System.Web.Http;
using System.Data.Entity;
using fIT.WebApi.Migrations;

using fIT.WebApi.Repositories.Context;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(fIT.WebApi.Startup))]

namespace fIT.WebApi
{
    public partial class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }


        /// <summary>
        ///  Will be fired once our server starts
        /// </summary>
        /// <param name="app">Composes the application for our Owin server. Parameter will be supplied by the host at run-time</param>
        public void Configuration(IAppBuilder app)
        {            
            //configure API routes
            HttpConfiguration config = new HttpConfiguration();

            //configure bearer authentification
            ConfigureOAuth(app);
            
            // Configure the db context and user manager to use a single instance per request
            //app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            WebApiConfig.Register(config);

            // allow CORS
            app.UseCors(CorsOptions.AllowAll);
            //wire up ASP.NET Web API to our Owin server pipeline
            app.UseWebApi(config);

            //initialise DB
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AuthContext, DBConfiguration>());

        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"), //token-generation-path
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                //Provider = new SimpleAuthorizationServerProvider(),
                //RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

        }
    }
}
