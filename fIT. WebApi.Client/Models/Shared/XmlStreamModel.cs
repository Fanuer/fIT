using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace fIT.WebApi.Client.Models.Shared
{
  public class XmlStreamModel: StreamModel
  {
    public XmlReader CreateXmlReader()
    {
      return XmlReader.Create(Stream);
    }
  }
}
