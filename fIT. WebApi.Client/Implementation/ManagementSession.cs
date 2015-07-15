using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Client.Intefaces;
using fIT.WebApi.Client.Models;
using fIT.WebApi.Client.Models.Shared;
using log4net;
using Newtonsoft.Json;

namespace fIT.WebApi.Client.Implementation
{
  public class ManagementSession:IManagementSession
  {
    #region Field
    private const string RefreshTokenPath = "/api/account/refreshtoken";
    public const string AccessTokenClaimType = "http://fit-bachelor.azurewebsites.net/api/accesstoken";
    public const string RefreshTokenClaimType = "http://fit-bachelor.azurewebsites.net/api/refreshtoken";
    public const string ExpiresClaimType = "http://fit-bachelor.azurewebsites.net/api/tokenexpires";
    internal const string ExternalClaimType = "http://fit-bachelor.azurewebsites.net/api/external";
    internal const string IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";

    protected static ILog Log = LogManager.GetLogger(typeof(ManagementSession));

    private WebRequestHandler handler;
    private HttpClient client;
    private string refreshToken;
    private readonly ManagementService service;
    private readonly string currentUsername;
    private DateTimeOffset expiresOn;

    #endregion

    #region Ctor
    internal ManagementSession(ManagementService service, string username, AuthenticationResultModel authentication)
    {
      this.service = service;
      this.Token = service.EncryptString(authentication.AccessToken);
      var accessToken = authentication.AccessToken;
      this.refreshToken = authentication.RefreshToken;
      this.currentUsername = username;

      Initialize(accessToken);
    }

    internal ManagementSession(ManagementService service, ClaimsIdentity identity)
    {
      this.service = service;
      this.Token = identity.FindFirst(AccessTokenClaimType).Value;
      var accessToken = service.DecryptString(identity.FindFirst(AccessTokenClaimType).Value);
      this.refreshToken = service.DecryptString(identity.FindFirst(RefreshTokenClaimType).Value);
      this.expiresOn = DateTimeOffset.Parse(identity.FindFirst(ExpiresClaimType).Value, CultureInfo.InvariantCulture);
      this.currentUsername = identity.FindFirst(ClaimTypes.Name).Value;

      Initialize(accessToken);
    }

    #endregion

    #region Methods
    private string QueryString(object data)
    {
      if (data == null) throw new ArgumentNullException("data");

      var parts = new List<string>();

      foreach (var p in data.GetType().GetProperties())
      {
        var value = p.GetGetMethod().Invoke(data, null);

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

    private void Initialize(string accessToken)
    {
      handler = new WebRequestHandler();
      handler.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

      client = new HttpClient(handler)
      {
        BaseAddress = new Uri(service.BaseUri),
        Timeout = TimeSpan.FromMinutes(15)
      };
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

      var tokenHandler = new JwtSecurityTokenHandler();
      try
      {
        var token = (JwtSecurityToken)tokenHandler.ReadToken(accessToken);
        expiresOn = token.ValidTo;
      }
      catch (Exception e)
      {
        expiresOn = DateTimeOffset.UtcNow.AddMinutes(30);
        Log.Error(e);
      }
    }

    public async Task PerformRefreshAsync()
    {
      string oldToken = Token;
      string newAccessToken = null;

      HttpResponseMessage response = await client.PostAsync(RefreshTokenPath, new ObjectContent(typeof(object), RefreshToken, new JsonMediaTypeFormatter()));
      if (response.IsSuccessStatusCode)
      {
        var result = await response.Content.ReadAsAsync<AuthenticationResultModel>();

        newAccessToken = result.AccessToken;
        this.refreshToken = result.RefreshToken;

        Token = service.EncryptString(newAccessToken);
      }

      if (newAccessToken == null)
      {
        Dispose();
        throw new ServerException(response);
      }

      var tokenHandler = new JwtSecurityTokenHandler();
      try
      {
        var token = (JwtSecurityToken)tokenHandler.ReadToken(newAccessToken);
        expiresOn = token.ValidTo;
      }
      catch (Exception e)
      {
        expiresOn = DateTimeOffset.UtcNow.AddMinutes(30);

        Log.Error(e);
      }

      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", newAccessToken);

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
        if (Log.IsDebugEnabled) Log.Debug(String.Format("GetAsync<{0}>({1}) -> {2}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));
        return await response.Content.ReadAsAsync<T>();
      }
      if (Log.IsInfoEnabled) Log.Info(String.Format("Failed GetAsync<{0}>({1}) -> {2}{3}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));

      throw new ServerException(response);
    }

    private async Task<HttpStatusCode> GetHttpStatusAsync(string url, params object[] args)
    {
      var response = await client.GetAsync(String.Format(url, args));
      return response.StatusCode;
    }

    private async Task<byte[]> GetBytesAsync(string url, params object[] args)
    {
      var response = await client.GetAsync(String.Format(url, args));
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("GetBytesAsync<{0}>({1}) -> {2}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));
        return await response.Content.ReadAsByteArrayAsync();
      }
      if (Log.IsInfoEnabled) Log.Info(String.Format("Failed GetBytesAsync<{0}>({1}) -> {2}{3}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response));

      throw new ServerException(response);
    }

    private async Task<StreamModel> GetStreamAsync(string url, params object[] args)
    {
      //ToDO wrap stream for service monitor

      var response = await client.GetAsync(String.Format(url, args));
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("GetStreamAsync<{0}>({1}) -> {2}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));

        var Stream = new StreamModel
        {
          Stream = await response.Content.ReadAsStreamAsync(),
          FileName = response.Content.Headers.ContentDisposition.FileName
        };

        return Stream;
      }

      if (Log.IsInfoEnabled) Log.Info(String.Format("Failed GetStreamAsync<{0}>({1}) -> {2}{3}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response));

      throw new ServerException(response);
    }

    private async Task<XmlStreamModel> GetXmlStreamAsync(string url, params object[] args)
    {
      //ToDO wrap stream for service monitor

      var response = await client.GetAsync(String.Format(url, args));
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("GetXmlStreamAsync<{0}>({1}) -> {2}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));

        var Stream = new XmlStreamModel
        {
          Stream = await response.Content.ReadAsStreamAsync(),
          FileName = response.Content.Headers.ContentDisposition.FileName
        };

        return Stream;
      }

      if (Log.IsInfoEnabled) Log.Info(String.Format("Failed GetXmlStreamAsync<{0}>({1}) -> {2}{3}", typeof(byte[]).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response));

      throw new ServerException(response);
    }

    private async Task PutAsJsonAsync<T>(T model, string url, params object[] args)
    {
      var response = await client.PutAsJsonAsync(String.Format(url, args), model);
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled)
          Log.Debug(String.Format("PutAsJsonAsync<{0}>({1}) -> {2}", typeof(T).Name, String.Format(url, args),
            await response.Content.ReadAsStringAsync()));
        response.Content.Dispose();
      }
      else
      {
        if (Log.IsInfoEnabled)
          Log.Info(String.Format("Failed PutAsJsonAsync<{0}>({1}) -> {2}{3}", typeof(T).Name,
            String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
        throw new ServerException(response);
      }
    }

    private async Task PutAsync(string url, params object[] args)
    {
      var response = await client.PutAsync(String.Format(url, args), new StringContent(String.Empty));
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("PutAsync({0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
        response.Content.Dispose();
      }
      else
      {
        if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PutAsync({0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
        throw new ServerException(response);
      }
    }

    private async Task DeleteAsync(string url, params object[] args)
    {
      var response = await client.DeleteAsync(String.Format(url, args));
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("DeleteAsync({0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
        response.Content.Dispose();
      }
      else
      {
        if (Log.IsInfoEnabled) Log.Info(String.Format("Failed DeleteAsync({0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
        throw new ServerException(response);
      }
    }

    private async Task<T> DeleteAsync<T>(string url, params object[] args)
    {
      var response = await client.DeleteAsync(String.Format(url, args));
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("DeleteAsync({0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
        return await response.Content.ReadAsAsync<T>();
      }
      if (Log.IsInfoEnabled) Log.Info(String.Format("Failed DeleteAsync({0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
      throw new ServerException(response);
    }

    private async Task DeleteAsJsonAsync<T>(T model, string url, params object[] args)
    {
      var message = new HttpRequestMessage(HttpMethod.Delete, JsonConvert.SerializeObject(model));
      var response = await client.SendAsync(message);
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("DeleteAsync({0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
        await response.Content.ReadAsAsync<T>();
      }
      if (Log.IsInfoEnabled) Log.Info(String.Format("Failed DeleteAsync({0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
      throw new ServerException(response);
    }

    private async Task<T> PostAsync<T>(HttpContent content, string url, params object[] args)
    {
      var response = await client.PostAsync(String.Format(url, args), content);
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("PostAsync<{0}>({1}) -> {2}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));
        return await response.Content.ReadAsAsync<T>();
      }
      else
      {
        if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PostAsync<{0}>({1}) -> {2}{3}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
        throw new ServerException(response);
      }
    }

    private async Task PostAsync(HttpContent content, string url, params object[] args)
    {
      var response = await client.PostAsync(String.Format(url, args), content);
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled)
          Log.Debug(String.Format("PostAsync(HttpContent, {0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
      }
      else
      {
        if (Log.IsInfoEnabled)
          Log.Info(String.Format("Failed PostAsync(HttpContent, {0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
        throw new ServerException(response);
      }
    }

    private async Task PostAsync(string url, params object[] args)
    {
      var response = await client.PostAsync(String.Format(url, args), new StringContent(String.Empty));
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("PostAsJsonAsync({0}) -> {1}", String.Format(url, args), await response.Content.ReadAsStringAsync()));
        response.Content.Dispose();
      }
      else
      {
        if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PutAsJsonAsync({0}) -> {1}{2}", String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));

        throw new ServerException(response);
      }
    }

    private async Task PostAsJsonAsync<T>(T model, string url, params object[] args)
    {
      var response = await client.PostAsJsonAsync(String.Format(url, args), model);
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("PostAsJsonAsync<{0}>({1}) -> {2}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));
        response.Content.Dispose();
      }
      else
      {
        if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PutAsJsonAsync<{0}>({1}) -> {2}{3}", typeof(T).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));

        throw new ServerException(response);
      }
    }

    private async Task<TResult> PutAsJsonReturnAsync<T, TResult>(T model, string url, params object[] args)
    {
      var response = await client.PutAsJsonAsync(String.Format(url, args), model);
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("PutAsJsonAsyncReturn<{0}, {1}>({2}) -> {3}", typeof(T).Name, typeof(TResult).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));

        return await response.Content.ReadAsAsync<TResult>();
      }

      if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PutAsJsonAsync<{0}, {1}>({2}) -> {3}{4}", typeof(T).Name, typeof(TResult).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
      throw new ServerException(response);
    }

    private async Task<TResult> PostAsJsonReturnAsync<T, TResult>(T model, string url, params object[] args)
    {
      var response = await client.PostAsJsonAsync(String.Format(url, args), model);
      if (response.IsSuccessStatusCode)
      {
        if (Log.IsDebugEnabled) Log.Debug(String.Format("PostAsJsonAsyncReturn<{0}, {1}>({2}) -> {3}", typeof(T).Name, typeof(TResult).Name, String.Format(url, args), await response.Content.ReadAsStringAsync()));
        return await response.Content.ReadAsAsync<TResult>();
      }
      else
      {
        if (Log.IsInfoEnabled) Log.Info(String.Format("Failed PostAsJsonAsyncReturn<{0}, {1}>({2}) -> {3}{4}", typeof(T).Name, typeof(TResult).Name, String.Format(url, args), await response.Content.ReadAsStringAsync(), response.ToString()));
        throw new ServerException(response);
      }
    }
    #endregion

    #endregion

    #region Properties

    public string Token { get; private set; }
    private string RefreshToken { get { return refreshToken; } }
    public DateTimeOffset ExpiresOn { get { return expiresOn; } }
    public string CurrentUsername { get { return currentUsername; }}

    public IAdminManagement Admins { get; set; }
    public IUserManagement Users { get; set; }
    #endregion
  }
}
