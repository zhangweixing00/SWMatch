using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterfacePlatform.Core;

namespace InterfacePlatform.Interface
{
    [System.Xml.Serialization.XmlRoot(ElementName = "xml")]
    public class MessageEventClick : MessageEvent
    {
        public string EventKey { get; set; }
        public static MessageEventClick GetInfo(string xml)
        {
            return XmlHelper.Deserializer<MessageEventClick>(xml);
        }
    }
}