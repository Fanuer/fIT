using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
      StatusCode = response.StatusCode;
    }

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
  }

}
