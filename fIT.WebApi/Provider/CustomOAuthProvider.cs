using fIT.WebApi.Entities;
using fIT.WebApi.Manager;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Provider
{
    /// <summary>
    /// Custom implementation of a OAuth Authorization Server Provider
    /// </summary>
    public class CustomOAuthProvider: OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// Validates a client thats tries to access the server.
        /// Exepts all clients, the api is the onlyclient available and do not allow adding additional clients
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// receiving the username and password from the request and validate them against our ASP.NET 2.1 Identity system
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //Allowing cross domain resources for external logins
            var allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            //Search user by username and password
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            // Email confirmation not implemented
            /*if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "User did not confirm email.");
                return;
            }*/

            // Generate claim and JWT-Ticket 
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");
            var ticket = new AuthenticationTicket(oAuthIdentity, null);

            //Transfer this identity to an OAuth 2.0 Bearer access ticket
            context.Validated(ticket);

        }
    }
}
