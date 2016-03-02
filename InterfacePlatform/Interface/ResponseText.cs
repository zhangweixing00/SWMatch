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
    public class ResponseText : BaseMessage
    {
        public ResponseText()
        {
            this.MsgType = ResponseMsgType.text.ToString().ToLower();
        }

        public ResponseText(BaseMessage info)
            : this()
        {
            this.FromUserName = info.ToUserName;
            this.ToUserName = info.FromUserName;
        }
        public override string ToXml()
        {
            this.CreateTime = (int)(DateTime.Now.Ticks);//重新更新
            return XmlHelper.Serializer(this);
        }
        /// <summary>
        /// 内容
        /// </summary>   
        public string Content { get; set; }
    }
}