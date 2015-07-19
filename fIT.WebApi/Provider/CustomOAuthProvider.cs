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
using fIT.WebApi.Entities.Enums;
using fIT.WebApi.Helpers;
using fIT.WebApi.Repository;
using fIT.WebApi.Repository.Interfaces;
using Microsoft.AspNet.Identity;

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
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
          string clientId;
          string clientSecret;
          Client client = null;

          if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
          {
            context.TryGetFormCredentials(out clientId, out clientSecret);
          }

          if (context.ClientId == null || context.ClientId == CustomRefreshTokenProvider.DUMMY_CLIENT)
          {
            //Remove the comments from the below line context.SetError, and invalidate context 
            //if you want to force sending clientId/secrects once obtain access tokens. 
            context.Validated();
            //context.SetError("invalid_clientId", "ClientId should be sent.");
            return ;
          }

          using (IRepository repo = new ApplicationRepository())
          {
            client = await repo.Clients.FindAsync(context.ClientId);
          }

          if (client == null)
          {
            context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
            return;
          }

          if (client.ApplicationType == ApplicationTypes.NativeConfidential)
          {
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
              context.SetError("invalid_clientId", "Client secret should be sent.");
              return ;
            }
            else
            {
              if (client.Secret != Helper.GetHash(clientSecret))
              {
                context.SetError("invalid_clientId", "Client secret is invalid.");
                return;
              }
            }
          }

          if (!client.Active)
          {
            context.SetError("invalid_clientId", "Client is inactive.");
            return;
          }

          context.OwinContext.Set("as:clientAllowedOrigin", client.AllowedOrigin);
          context.OwinContext.Set("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

          context.Validated();
        }
        
        /// <summary>
        /// receiving the username and password from the request and validate them against our ASP.NET 2.1 Identity system
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //Allowing cross domain resources for external logins
          var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin") ?? "*";
          context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
          
            //Search user by username and password
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
          
            // Generate claim and JWT-Ticket 
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");
            oAuthIdentity.AddClaim(new Claim("userId", oAuthIdentity.GetUserId()));

            var ticket = new AuthenticationTicket(oAuthIdentity, null);

            //Transfer this identity to an OAuth 2.0 Bearer access ticket
            context.Validated(ticket);
            
        }
      
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
          foreach (var property in context.Properties.Dictionary)
          {
            context.AdditionalResponseParameters.Add(property.Key, property.Value);
          }
          return Task.FromResult<object>(null);
        }

      public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
      {
        var originalClient = context.Ticket.Properties.Dictionary.ContainsKey("as:client_id") ? context.Ticket.Properties.Dictionary["as:client_id"] : "";
        var currentClient = context.ClientId ?? "";

        if (originalClient != currentClient)
        {
          context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
          return Task.FromResult<object>(null);
        }

        // Change auth ticket for refresh token requests
        var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
        newIdentity.AddClaim(new Claim("newClaim", "newValue"));

        var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
        context.Validated(newTicket);

        return Task.FromResult<object>(null);
      }
    }
}
