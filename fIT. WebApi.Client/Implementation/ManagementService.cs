using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using fIT.WebApi.Client.Intefaces;
using fIT.WebApi.Client.Models;
using Newtonsoft.Json;

namespace fIT.WebApi.Client.Implementation
{
  public class ManagementService : IManagementService, IDisposable
  {
    #region Field
    private const string API_PATH = "/api/";
    private const string LOGIN_PATH = API_PATH + "accounts/login";
    private const string PASSWORD_PATH = API_PATH + "accounts/changepassword";
    private Dictionary<string, IManagementSession> sessions;

    private readonly WebRequestHandler handler;
    private readonly HttpClient client;
    private bool disposed;

    private readonly RijndaelManaged algorithm;
    private readonly byte[] rgbKey;
    private readonly byte[] rgbIv;

    #endregion

    #region Ctor

    public ManagementService(string baseUri, string tokenEncryptionPassphrase = null)
    {
      handler = new WebRequestHandler();
      handler.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

      BaseUri = baseUri;
      client = new HttpClient(handler)
      {
        BaseAddress = new Uri(baseUri)
      };

      algorithm = new RijndaelManaged();
      var rdb = new Rfc2898DeriveBytes(tokenEncryptionPassphrase ?? "STATIC", Encoding.Unicode.GetBytes("Some salt ..."));
      rgbKey = rdb.GetBytes(algorithm.KeySize >> 3);
      rgbIv = rdb.GetBytes(algorithm.BlockSize >> 3);

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

    public async Task<IManagementSession> GetSessionAsync(ClaimsIdentity identity)
    {
      var claim = identity.FindFirst(ManagementSession.AccessTokenClaimType);
      if (claim == null) return null;

      string token = claim.Value;

      IManagementSession session;

      if (!sessions.TryGetValue(token, out session))
      {
        try
        {
          session = new ManagementSession(this, identity);
          if (await session.Validate())
          {
            if (sessions != null) sessions[token] = session;
          }
          else
          {
            session.Dispose();
            session = null;
          }
        }
        catch { }
      }

      return session;
    }

    public async Task<IManagementSession> LoginAsync(string upn, string password)
    {
        HttpResponseMessage response = await client.PostAsync(LOGIN_PATH, new ObjectContent(typeof(object), new { Upn = upn, Password = password, grant_type="password" }, new JsonMediaTypeFormatter()));
        if (response.IsSuccessStatusCode)
        {
          var result = await response.Content.ReadAsStringAsync();
          var resultEntries = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
          var accessResult = new AuthenticationResultModel(resultEntries);
          var session = new ManagementSession(this, upn, accessResult);

          sessions[session.Token] = session;

          return session;
        }
        throw new ServerException(response);
      }

    public async Task UpdatePasswordAsync(string upn, string oldPassword, string newPassword)
    {
        HttpResponseMessage response = await client.PutAsync(PASSWORD_PATH, new ObjectContent(typeof(object), new { Upn = upn, OldPassword = oldPassword, NewPassword = newPassword }, new JsonMediaTypeFormatter()));
        if (response.IsSuccessStatusCode)
        {
          var result = await response.Content.ReadAsStringAsync();
        }
        else
        {
          throw new ServerException(response);
        }
      
    }

    public string EncryptString(String base64String)
    {
      return Convert.ToBase64String(Encrypt(new UTF8Encoding(false).GetBytes(base64String)));
    }

    public string DecryptString(String base64String)
    {
      return new UTF8Encoding(false).GetString(Decrypt(Convert.FromBase64String(base64String)));
    }

    public byte[] Encrypt(byte[] bytes)
    {
      var transform = algorithm.CreateEncryptor(rgbKey, rgbIv);

      using (var buffer = new MemoryStream())
      using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
      {
        stream.Write(bytes, 0, bytes.Length);
        stream.FlushFinalBlock();

        return buffer.ToArray();
      }
    }

    public byte[] Decrypt(byte[] input)
    {
      var transform = algorithm.CreateDecryptor(rgbKey, rgbIv);

      using (var buffer = new MemoryStream(input, false))
      using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
      using (var target = new MemoryStream())
      {
        stream.CopyTo(target);

        return target.ToArray();
      }
    }

    #endregion

    #region Properties
    internal string BaseUri { get; private set; }

    #endregion
  }
}
