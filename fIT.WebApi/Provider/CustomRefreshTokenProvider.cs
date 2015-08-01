using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using fIT.WebApi.Entities;
using fIT.WebApi.Helpers;
using fIT.WebApi.Repository;
using fIT.WebApi.Repository.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Infrastructure;

namespace fIT.WebApi.Provider
{
  public class CustomRefreshTokenProvider : IAuthenticationTokenProvider
  {
    internal const string DUMMY_CLIENT = "MyClient";
    private const string IS_REFREHTOKEN_EXPIRED_NAME = "IsRefreshTokenExpired";

    public void Create(AuthenticationTokenCreateContext context)
    {
      this.CreateAsync(context).Wait();
    }

    public async Task CreateAsync(AuthenticationTokenCreateContext context)
    {
      if (!context.OwinContext.Environment.ContainsKey(IS_REFREHTOKEN_EXPIRED_NAME) || (bool)context.OwinContext.Environment[IS_REFREHTOKEN_EXPIRED_NAME])
      {
        bool result = false;
        var refreshTokenId = Guid.NewGuid().ToString("n");
        var clientId = context.Ticket.Properties.Dictionary.ContainsKey("as:client_id") ? context.Ticket.Properties.Dictionary["as:client_id"] : DUMMY_CLIENT;


        var refreshTokenLifetime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime") ?? "30";
        var token = new RefreshToken()
        {
          Id = Helper.GetHash(refreshTokenId),
          ClientId = clientId,
          Subject = context.Ticket.Identity.Name,
          IssuedUtc = DateTime.UtcNow,
          ExpiresUtc = DateTime.UtcNow.AddMinutes(Double.Parse(refreshTokenLifetime))
        };
        context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
        context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;
        // Add current User Id
        context.Ticket.Properties.Dictionary.Add("UserId", context.Ticket.Identity.GetUserId());

        if (!context.Ticket.Properties.Dictionary.ContainsKey("as:client_id"))
        {
          context.Ticket.Properties.Dictionary.Add("as:client_id", clientId);
        }
        else
        {
          context.Ticket.Properties.Dictionary["as:client_id"] = clientId;
        }
        

        token.ProtectedTicket = context.SerializeTicket();

        using (IRepository rep = new ApplicationRepository())
        {
          result = await rep.RefreshTokens.AddAsync(token);
        }
        if (result)
        {
          context.SetToken(refreshTokenId);
        }
      }
    }

    public void Receive(AuthenticationTokenReceiveContext context)
    {
      this.ReceiveAsync(context).Wait();
    }

    public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
    {
      try
      {
        var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin") ?? "*";
        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

        var hashedTokenId = Helper.GetHash(context.Token);
        using (IRepository rep = new ApplicationRepository())
        {
          var refreshToken = await rep.RefreshTokens.FindAsync(hashedTokenId);

          if (refreshToken != null)
          {
            //Get protectedTicket from refreshToken class
            context.DeserializeTicket(refreshToken.ProtectedTicket);
            var result = await rep.RefreshTokens.RemoveAsync(hashedTokenId);
          }
        }

      }
      catch (Exception e)
      {
        
        throw e;
      }
    }
  }
}