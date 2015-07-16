using System;
using System.Net;
using fIT.WebApi.Client.Implementation;
using fIT.WebApi.Client.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fIT.WebApi.Tests
{
  [TestClass]
  public partial class ClientTests
  {
    #region Field
#if DEBUG
    private const string ServiceUrl = "http://localhost:62816/";
#else
      private const string ServiceUrl = "http://fit-bachelor.azurewebsites.net/";
#endif

    private const string Username = "Stefan";
    private const string Password = "Test1234";

    #endregion

    [TestInitialize]
    public void Setup()
    {
      // ignore invalid ssl certs
      ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
    }

    [TestMethod]
    public void Login()
    {
      try
      {
        using (var service = new ManagementService(ServiceUrl))
        using (var session = service.LoginAsync(Username, Password).Result)
        { 
        }
      }
      catch (Exception e)
      {
        
        throw e;
      }
      
    }

    [TestMethod]
    public void Ping()
    {
      try
      {
        using (var service = new ManagementService(ServiceUrl))
        {
          Assert.IsTrue(service.Ping().Result);
        }
      }
      catch (Exception e)
      {

        throw e;
      }

    }


    [TestMethod]
    public void PerformRefresh()
    {
      using (var service = new ManagementService(ServiceUrl))
      using (var session = service.LoginAsync(Username, Password).Result)
      {
        var oldValue = session.Token;
        session.PerformRefreshAsync().Wait();
        var newValue = session.Token;
        Assert.AreNotEqual(oldValue, newValue);
      }
    }

    [TestMethod]
    public void FailLoginInvalidUsername()
    {
      using (var service = new ManagementService(ServiceUrl))
      {
        try
        {
          using (var session = service.LoginAsync("xyz", Password).Result)
          {
          }

          Assert.Fail();
        }
        catch (AggregateException e)
        {
          Assert.IsTrue(e.InnerException is ServerException);
        }
      }
    }
  }
}
