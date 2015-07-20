using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace fIT.WebApi.Client.Models
{
  public abstract class ManagementApiException:Exception
  {
    protected ManagementApiException() { }

    protected ManagementApiException(string message){ }
  }

  public class ServerException : ManagementApiException
  {
    public ServerException() { }

    public ServerException(HttpResponseMessage response)
      : base(ResolveMessage(response))
    {
      var resolved = response.Content.ReadAsAsync<ErrorContent>().Result;
      this.ModelState = resolved.ModelState; 
      this.StatusCode = response.StatusCode;
    }

    public ModelStateDictionary ModelState{ get; set; }

    public HttpStatusCode StatusCode { get; private set; }

    private static string ResolveMessage(HttpResponseMessage response)
    {
      try
      {
        return response.Content.ReadAsAsync<ErrorMessage>().Result.Message;
      }
      catch { }

      return response.ToString();
    }

    public class ErrorMessage
    {
      public string Message;
    }

    public class ErrorContent: ErrorMessage
    {
      public ModelStateDictionary ModelState { get; set; }
    }

  }

}
