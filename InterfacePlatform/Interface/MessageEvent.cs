using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterfacePlatform.Core;

namespace InterfacePlatform.Interface
{
    [System.Xml.Serialization.XmlRoot(ElementName = "xml")]
    public class MessageEvent : BaseMessage
    {
        public string Event { get; set; }
        public static MessageEvent GetInfo(string xml)
        {
            return XmlHelper.Deserializer<MessageEvent>(xml);
        }
    }
}