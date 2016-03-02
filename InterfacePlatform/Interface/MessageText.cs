using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterfacePlatform.Core;

namespace InterfacePlatform.Interface
{
    /// <summary>
    /// 回复文本消息
    /// </summary>
    [System.Xml.Serialization.XmlRoot(ElementName = "xml")]
    public class MessageText : BaseMessage
    {
        public MessageText()
        {
            this.MsgType = ResponseMsgType.text.ToString().ToLower();
        }
        /// <summary>
        /// 内容
        /// </summary>        
        public string Content { get; set; }

        public static MessageText GetInfo(string xml)
        {
            return XmlHelper.Deserializer<MessageText>(xml);
        }
    }
}