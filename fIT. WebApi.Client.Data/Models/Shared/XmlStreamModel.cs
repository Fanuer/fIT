using System.Xml;

namespace fIT.WebApi.Client.Data.Models.Shared
{
  public class XmlStreamModel: StreamModel
  {
    public XmlReader CreateXmlReader()
    {
      return XmlReader.Create(Stream);
    }
  }
}
