using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Tokens;

namespace fIT.WebApi.Provider
{
    /// <summary>
    /// Authorization- and Resource Server
    /// 
    /// Authorization Server:
    /// Gets and parses Authorization requests and issues them to the OWIN middleware
    /// Resource Server (Audience) aka client:
    /// Requests JWT from the Authorization Server. Must be registered to the Authorization Server.
    /// </summary>
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        /// <summary>
        /// Issurer is our API, which acts as Authorization- and Resource Server.
        /// Is empty, because we allow all clients
        /// </summary>
        private readonly string _issuer = string.Empty;

        /// <summary>
        /// Contruktor
        /// </summary>
        /// <param name="issuer">calling Issuer</param>
        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            //Audience = resource Server
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            string symmetricKeyAsBase64 = ConfigurationManager.AppSettings["as:AudienceSecret"];
            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
            // symmetric key
            var signingKey = new HmacSigningCredentials(keyByteArray);
            // issue Date
            var issued = data.Properties.IssuedUtc;
            // expire date
            var expires = data.Properties.ExpiresUtc;
            // access token
            var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);
            // Token Creater (serialize Token-object to string)
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}
