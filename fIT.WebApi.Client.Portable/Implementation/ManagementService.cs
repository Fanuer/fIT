using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fIT.WebApi.Client.Data.Models.Shared;
using fIT.WebApi.Client.Portable.Helper;
using Newtonsoft.Json;

namespace fIT.WebApi.Client.Portable.Implementation
{
    public class ManagementService : IManagementService, IDisposable
    {
        #region Field
        private const string API_PATH = "/api/";
        private const string LOGIN_PATH = API_PATH + "accounts/login";
        private const string PASSWORD_PATH = API_PATH + "accounts/changepassword";
        private Dictionary<string, IManagementSession> sessions;

        private readonly HttpClientHandler handler;
        private readonly HttpClient client;
        private bool disposed;
        
        #endregion

        #region Ctor

        public ManagementService(string baseUri, string tokenEncryptionPassphrase = null, ClientInformation clientInformation = null)
        {
            
            handler = new HttpClientHandler();

            BaseUri = baseUri;
            client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUri)
            };

            this.ClientInformation = clientInformation ?? new ClientInformation();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            sessions = new Dictionary<string, IManagementSession>();

        }

        ~ManagementService()
        {
            Dispose(false);
        }

        #endregion

        #region Methods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposed) return;

            if (!disposing)
            {
                try
                {
                    client.CancelPendingRequests();
                }
                catch { }

                client.Dispose();
                handler.Dispose();

                IManagementSession[] currentSessions = sessions.Values.ToArray();
                sessions = null;

                foreach (IManagementSession session in currentSessions)
                {
                    session.Dispose();
                }
            }

            disposed = true;
        }

        internal void SessionClosed(IManagementSession managementSession)
        {
            if (sessions == null) return;

            lock (sessions)
            {
                sessions.Remove(managementSession.Token);
            }
        }

        internal void AccessTokenChanged(string oldAccessToken, string accessToken, IManagementSession managementSession)
        {
            lock (sessions)
            {
                sessions.Remove(oldAccessToken);
                sessions.Add(accessToken, managementSession);
            }
        }

        //public async Task<IManagementSession> GetSessionAsync(ClaimsIdentity identity)
        //{
        //  var claim = identity.FindFirst(ManagementSession.AccessTokenClaimType);
        //  if (claim == null) return null;

        //  string token = claim.Value;

        //  IManagementSession session;

        //  if (!sessions.TryGetValue(token, out session))
        //  {
        //    try
        //    {
        //      session = new ManagementSession(this, identity);
        //      if (await session.Validate())
        //      {
        //        if (sessions != null) sessions[token] = session;
        //      }
        //      else
        //      {
        //        session.Dispose();
        //        session = null;
        //      }
        //    }
        //    catch { }
        //  }

        //  return session;
        //}

        public async Task<IManagementSession> LoginAsync(string username, string password)
        {
            const string CONTENT = "username={0}&password={1}&grant_type=password";
            var stringContent = String.Format(CONTENT, username, password);
            stringContent = this.ClientInformation.AddClientData(stringContent);
            var content = new StringContent(stringContent);

            //var content = new ObjectContent(typeof(object), new { username, Password = password, grant_type = "password" }, new JsonMediaTypeFormatter());
            HttpResponseMessage response = await client.PostAsync(LOGIN_PATH, content);

            if (response.IsSuccessStatusCode)
            {
                var accessResult = await response.Content.ReadAsAsync<AuthenticationResultModel>();
                var session = new ManagementSession(this, username, accessResult);

                sessions[session.Token] = session;

                return session;
            }
            throw new ServerException(response);
        }

        public async Task UpdatePasswordAsync(string username, string oldPassword, string newPassword)
        {
            var contentObject = new {Upn = username, OldPassword = oldPassword, NewPassword = newPassword};
            var content = new StringContent(JsonConvert.SerializeObject(contentObject));

            HttpResponseMessage response = await client.PutAsync(PASSWORD_PATH, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new ServerException(response);
            }

        }

        /// <summary>
        /// Sends a Request to the server to verify the connection
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PingAsync()
        {
            var result = await this.client.GetAsync("/api/accounts/Ping");
            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Registers a Users 
        /// </summary>
        /// <param name="model">data of the new user</param>
        /// <returns></returns>
        public async Task RegisterAsync(CreateUserModel model)
        {
            var json = JsonConvert.SerializeObject(model);

            HttpResponseMessage response = null;
            try
            {
                response = await client.PostAsync("/api/accounts/Register", new StringContent(json, Encoding.UTF8, "application/json"));
            }
            catch (Exception)
            {
                
                throw;
            }
            
            if (response.IsSuccessStatusCode)
            {
                response.Content.Dispose();
            }
            else
            {
              var result = await response.Content.ReadAsAsync<HttpError>();
                throw new ServerException(response);
            }
        }
        #endregion

        #region Properties
        internal string BaseUri { get; private set; }
        internal ClientInformation ClientInformation { get; private set; }
        #endregion
    }
}
