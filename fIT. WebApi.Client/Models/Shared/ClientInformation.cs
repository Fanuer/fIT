using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Client.Models.Shared
{
  public class ClientInformation
  {
    #region Field
    public string Id { get; set; }
    public string Secret { get; set; }

    internal const string CLIENT_ID_CONTENT = "&client_id={0}";
    internal const string CLIENT_SECRET_CONTENT = "&client_secret={0}";

    #endregion

    
    internal string AddClientData(string content)
    {
      if (String.IsNullOrWhiteSpace(content))
      {
        content += "";
      }
      if (!String.IsNullOrWhiteSpace(Id))
      {
        content += String.Format(CLIENT_ID_CONTENT, Id);
      }
      if (!String.IsNullOrWhiteSpace(Secret))
      {
        content += String.Format(CLIENT_SECRET_CONTENT, Secret);
      }
      return content;
    }
  }
}
