using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using fIT.WebApi.Client.Data.Intefaces;
using fIT.WebApi.Client.Data.Models.Account;
using fIT.WebApi.Client.Data.Models.Exceptions;
using fIT.WebApi.Client.Data.Models.Shared;
using fIT.WebApi.Client.Portable.Helper;
using Newtonsoft.Json;

namespace fIT.WebApi.Client.Portable.Implementation
{
    public partial class ManagementSession : IManagementSession
    {
        #region Field
        private const string RefreshTokenPath = "/api/accounts/login";
        public const string AccessTokenClaimType = "http://fit-bachelor.azurewebsites.net/api/accesstoken";
        public const string RefreshTokenClaimType = "http://fit-bachelor.azurewebsites.net/api/refreshtoken";
        public const string ExpiresClaimType = "http://fit-bachelor.azurewebsites.net/api/tokenexpires";
        internal const string ExternalClaimType = "http://fit-bachelor.azurewebsites.net/api/external";
        internal const string IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";

        private HttpClientHandler handler;
        private HttpClient client;
        private string refreshToken;
        private readonly ManagementService service;
        private DateTimeOffset expiresOn;

        #endregion

        #region Ctor
        internal ManagementSession(ManagementService service, string username, AuthenticationResultModel authentication)
        {
            this.service = service;
            this.Token = authentication.AccessToken;
            var accessToken = authentication.AccessToken;
            CurrentUserId = authentication.UserId;
            this.refreshToken = authentication.RefreshToken;
            this.CurrentUserName = username;

            Initialize(accessToken, new DateTimeOffset(authentication.ExpireDate));
        }

        //internal ManagementSession(ManagementService service, ClaimsIdentity identity)
        //{
        //    this.service = service;
        //    this.Token = identity.FindFirst(AccessTokenClaimType).Value;
        //    var accessToken = service.DecryptString(identity.FindFirst(AccessTokenClaimType).Value);
        //    this.refreshToken = service.DecryptString(identity.FindFirst(RefreshTokenClaimType).Value);
        //    this.expiresOn = DateTimeOffset.Parse(identity.FindFirst(ExpiresClaimType).Value, CultureInfo.InvariantCulture);
        //    this.currentUsername = identity.FindFirst(ClaimTypes.Name).Value;

        //    Initialize(accessToken);
        //}

        #endregion

        #region Methods
        private string QueryString(object data)
        {
            if (data == null) throw new ArgumentNullException("data");

            var parts = new List<string>();

            foreach (var p in data.GetType().GetRuntimeProperties().Select(x => x.GetMethod))
            {
                var value = p.Invoke(data, null);

                if (value != null)
                {
                    string sVal = String.Empty;
                    if (value is bool)
                    {
                        sVal = ((bool)value) ? "true" : "false";
                    }
                    else if (value is DateTime)
                    {
                        sVal = ((DateTime)value).ToString("yyyyMMdd");
                    }
                    else
                    {
                        sVal = WebUtility.UrlEncode(value.ToString());
                    }


                    parts.Add(p.Name + "=" + sVal);
                }
            }

            return parts.Count > 0 ? "?" + String.Join("&", parts) : String.Empty;
        }

        private void Initialize(string accessToken, DateTimeOffset? initialExpireTime = null)
        {
            handler = new HttpClientHandler();

            client = new HttpClient(handler)
            {
                BaseAddress = new Uri(service.BaseUri),
                Timeout = TimeSpan.FromMinutes(15)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            if (initialExpireTime.HasValue)
            {
                expiresOn = initialExpireTime.Value;
            }
            else
            {
                expiresOn = DateTimeOffset.UtcNow.AddMinutes(30);
            }
        }

        public async Task PerformRefreshAsync()
        {
            string oldToken = Token;
            const string REFRESHCONTENT = "grant_type=refresh_token&refresh_token={0}&client_id=MyClient";
            var stringContent = String.Format(String.Format(REFRESHCONTENT, this.RefreshToken));
            stringContent = this.service.ClientInformation.AddClientData(stringContent);
            var content = new StringContent(stringContent);

            HttpResponseMessage response = await client.PostAsync(RefreshTokenPath, content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<AuthenticationResultModel>();
                this.refreshToken = result.RefreshToken;
                Token = result.AccessToken;
                expiresOn = new DateTimeOffset(result.ExpireDate);
            }

            if (Token == null)
            {
                Dispose();
                throw new ServerException(response);
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Token);

            service.AccessTokenChanged(oldToken, Token, this);
        }

        public void Dispose()
        {
            // try cancel pending requests
            try
            {
                client.CancelPendingRequests();
            }
            catch { }

            // try dispose
            try
            {
                client.Dispose();
            }
            catch { }

            client = null;

            try
            {
                handler.Dispose();
            }
            catch { }

            handler = null;

            // remove from service
            service.SessionClosed(this);
        }

        #region Logging HttpClient wrapper
        private async Task<T> GetAsync<T>(string url, params object[] args)
        {
            var response = await client.GetAsync(String.Format(url, args));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }

            throw new ServerException(response);
        }

        private async Task<HttpStatusCode> GetHttpStatusAsync(string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.GetAsync(String.Format(url, args));
            return response.StatusCode;
        }

        private async Task<byte[]> GetBytesAsync(string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.GetAsync(String.Format(url, args));
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("GetBytesAsync<{0}>({1}) -> {2}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));
                return await response.Content.ReadAsByteArrayAsync();
            }
            //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed GetBytesAsync<{0}>({1}) -> {2}{3}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response));

            throw new ServerException(response);
        }

        private async Task<StreamModel> GetStreamAsync(string url, params object[] args)
        {
            //ToDO wrap stream for service monitor

            await CheckForRefreshRequirement();
            var response = await client.GetAsync(String.Format(url, args));
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("GetStreamAsync<{0}>({1}) -> {2}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));

                var Stream = new StreamModel
                {
                    Stream = await response.Content.ReadAsStreamAsync(),
                    FileName = response.Content.Headers.ContentDisposition.FileName
                };

                return Stream;
            }

            //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed GetStreamAsync<{0}>({1}) -> {2}{3}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response));

            throw new ServerException(response);
        }

        private async Task<XmlStreamModel> GetXmlStreamAsync(string url, params object[] args)
        {
            //ToDO wrap stream for service monitor

            await CheckForRefreshRequirement();
            var response = await client.GetAsync(String.Format(url, args));
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("GetXmlStreamAsync<{0}>({1}) -> {2}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));

                var Stream = new XmlStreamModel
                {
                    Stream = await response.Content.ReadAsStreamAsync(),
                    FileName = response.Content.Headers.ContentDisposition.FileName
                };

                return Stream;
            }

            //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed GetXmlStreamAsync<{0}>({1}) -> {2}{3}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response));

            throw new ServerException(response);
        }

        private async Task PutAsJsonAsync<T>(T model, string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.PutAsJsonAsync(String.Format(url, args), model);
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled)
                //Log.Debug(String.Format("PutAsJsonAsync<{0}>({1}) -> {2}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));
                response.Content.Dispose();
            }
            else
            {
                //if (Log.IsInfoEnabled)
                //Log.Info(String.Format("Failed PutAsJsonAsync<{0}>({1}) -> {2}{3}", typeof(T).Name,
                //String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
                throw new ServerException(response);
            }
        }

        private async Task PutAsync(string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.PutAsync(String.Format(url, args), new StringContent(String.Empty));
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("PutAsync({0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
                response.Content.Dispose();
            }
            else
            {
                //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PutAsync({0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
                throw new ServerException(response);
            }
        }

        private async Task DeleteAsync(string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.DeleteAsync(String.Format(url, args));
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("DeleteAsync({0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
                response.Content.Dispose();
            }
            else
            {
                //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed DeleteAsync({0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
                throw new ServerException(response);
            }
        }

        private async Task<T> DeleteAsync<T>(string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.DeleteAsync(String.Format(url, args));
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("DeleteAsync({0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
                return await response.Content.ReadAsAsync<T>();
            }
            //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed DeleteAsync({0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
            throw new ServerException(response);
        }

        private async Task<T> PostAsync<T>(HttpContent content, string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.PostAsync(String.Format(url, args), content);
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("PostAsync<{0}>({1}) -> {2}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));
                return await response.Content.ReadAsAsync<T>();
            }
            else
            {
                //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PostAsync<{0}>({1}) -> {2}{3}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
                throw new ServerException(response);
            }
        }

        private async Task PostAsync(HttpContent content, string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.PostAsync(String.Format(url, args), content);
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled)
                //Log.Debug(String.Format("PostAsync(HttpContent, {0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
            }
            else
            {
                //if (Log.IsInfoEnabled)
                //Log.Info(String.Format("Failed PostAsync(HttpContent, {0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
                throw new ServerException(response);
            }
        }

        private async Task PostAsync(string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.PostAsync(String.Format(url, args), new StringContent(String.Empty));
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("PostAsJsonAsync({0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
                //response.Content.Dispose();
            }
            else
            {
                //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PutAsJsonAsync({0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));

                throw new ServerException(response);
            }
        }

        private async Task PostAsJsonAsync<T>(T model, string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.PostAsJsonAsync(String.Format(url, args), model);
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("PostAsJsonAsync<{0}>({1}) -> {2}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));
                response.Content.Dispose();
            }
            else
            {
                //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PutAsJsonAsync<{0}>({1}) -> {2}{3}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));

                throw new ServerException(response);
            }
        }

        private async Task<TResult> PutAsJsonReturnAsync<T, TResult>(T model, string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.PutAsJsonAsync(String.Format(url, args), model);
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("PutAsJsonAsyncReturn<{0}, {1}>({2}) -> {3}", typeof(T).Name, typeof(TResult).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));

                return await response.Content.ReadAsAsync<TResult>();
            }

            //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PutAsJsonAsync<{0}, {1}>({2}) -> {3}{4}", typeof(T).Name, typeof(TResult).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
            throw new ServerException(response);
        }

        private async Task<TResult> PostAsJsonReturnAsync<T, TResult>(T model, string url, params object[] args)
        {
            await CheckForRefreshRequirement();
            var response = await client.PostAsJsonAsync(String.Format(url, args), model);
            if (response.IsSuccessStatusCode)
            {
                //if (Log.IsDebugEnabled) Log.Debug(String.Format("PostAsJsonAsyncReturn<{0}, {1}>({2}) -> {3}", typeof(T).Name, typeof(TResult).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));
                return await response.Content.ReadAsAsync<TResult>();
            }
            else
            {
                //if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PostAsJsonAsyncReturn<{0}, {1}>({2}) -> {3}{4}", typeof(T).Name, typeof(TResult).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
                throw new ServerException(response);
            }
        }

        private async Task CheckForRefreshRequirement()
        {
            if (!String.IsNullOrWhiteSpace(this.RefreshToken) && DateTimeOffset.UtcNow > this.ExpiresOn.Subtract(TimeSpan.FromMinutes(30)))
            {
                try
                {
                    await this.PerformRefreshAsync();
                }
                catch (ServerException e)
                {
                    throw e;
                }
            }
        }
        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Encrypted Access Token to use as Id
        /// </summary>
        public string Token { get; private set; }
        /// <summary>
        /// Current Refresh Token
        /// </summary>
        private string RefreshToken { get { return refreshToken; } }
        /// <summary>
        /// Access Token Expire Date
        /// </summary>
        public DateTimeOffset ExpiresOn { get { return expiresOn; } }
        /// <summary>
        /// Current Username
        /// </summary>
        public string CurrentUserName { get; private set; }
        public Guid CurrentUserId { get; private set; }
        #endregion
    }
}
