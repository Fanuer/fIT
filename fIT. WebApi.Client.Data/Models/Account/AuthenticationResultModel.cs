using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace fIT.WebApi.Client.Data.Models.Account
{
    public class AuthenticationResultModel
    {
        #region Field

        private const string ACCESS_TOKEN = "access_token";
        private const string REFRESH_TOKEN = "refresh_token";
        private const string EXPIREDATE = ".expires";
        private const string ISSUEDATE = ".issued";
        private const string TOKEN_TYPE = "token_type";
        private const string CLIENT_ID = "as:client_id";
        #endregion

        #region Ctor

        public AuthenticationResultModel(): this("")
        {
            
        }

        public AuthenticationResultModel(string accessToken = "", string refreshToken = "", string tokenType = "bearer", DateTime expireDate = default(DateTime), DateTime issueDate = default(DateTime), string clientId = "")
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            TokenType = tokenType;
            ExpireDate = expireDate;
        }

        public AuthenticationResultModel(IDictionary<string, string> httpContentResults):this()
        {
            if (httpContentResults == null)
            {
                throw new ArgumentNullException("httpContentResults");
            }

            if (httpContentResults.ContainsKey(ACCESS_TOKEN))
            {
                AccessToken = httpContentResults[ACCESS_TOKEN];
            }
            if (httpContentResults.ContainsKey(REFRESH_TOKEN))
            {
                RefreshToken = httpContentResults[REFRESH_TOKEN];
            }
            if (httpContentResults.ContainsKey(TOKEN_TYPE))
            {
                TokenType = httpContentResults[TOKEN_TYPE];
            }
            if (httpContentResults.ContainsKey(CLIENT_ID))
            {
                ClientId = httpContentResults[CLIENT_ID];
            }
            if (httpContentResults.ContainsKey(ISSUEDATE))
            {
                IssueDate = DateTime.Parse(httpContentResults[ISSUEDATE]);
                //IssueDate = Convert.ToDateTime(httpContentResults[ISSUEDATE]);
            }
            if (httpContentResults.ContainsKey(EXPIREDATE))
            {
                ExpireDate = DateTime.Parse(httpContentResults[EXPIREDATE]);
            }

        }
        #endregion

        #region Methods

        #endregion

        #region Properties
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty(".expires")]
        public DateTime ExpireDate { get; set; }
        [JsonProperty(".issued")]
        public DateTime IssueDate { get; set; }
        [JsonProperty("as:client_id")]
        public string ClientId { get; set; }

        public Guid UserId { get; set; }
        #endregion
    }
}
