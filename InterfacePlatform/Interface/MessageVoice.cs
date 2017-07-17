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
    public class MessageVoice : BaseMessage
    {
        public MessageVoice()
        {
            this.MsgType = ResponseMsgType.Voice.ToString().ToLower();
        }
        /// <summary>
        /// 内容
        /// </summary>        
        public string Recognition { get; set; }

        public static MessageVoice GetInfo(string xml)
        {
            return XmlHelper.Deserializer<MessageVoice>(xml);
        }
    }
}