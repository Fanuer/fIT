using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using fIT.WebApi.Entities;
using fIT.WebApi.Helpers;
using fIT.WebApi.Repository;
using fIT.WebApi.Repository.Interfaces;
using Microsoft.Owin.Security.Infrastructure;

namespace fIT.WebApi.Provider
{
  public class CustomRefreshTokenProvider: IAuthenticationTokenProvider
  {
    private const string DUMMY_CLIENT = "MyClient";

    public void Create(AuthenticationTokenCreateContext context)
    {
      this.CreateAsync(context).Wait();
    }

    public async Task CreateAsync(AuthenticationTokenCreateContext context)
    {
      bool result = false;
      var refreshTokenId = Guid.NewGuid().ToString("n");
      var token = new RefreshToken()
      {
        Id = Helper.GetHash(refreshTokenId),
        ClientId = context.Ticket.Properties.Dictionary["as:client_id"] ?? DUMMY_CLIENT,
        Subject = context.Ticket.Identity.Name,
        IssuedUtc = DateTime.UtcNow,
        ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime")))
      };
      context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
      context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

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

    public void Receive(AuthenticationTokenReceiveContext context)
    {
      throw new NotImplementedException();
    }

    public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
    {
      var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
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
  }
}