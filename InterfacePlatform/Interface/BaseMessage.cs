using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using InterfacePlatform.Core;

namespace InterfacePlatform.Interface
{
    /// <summary>
    /// 基础消息内容
    /// </summary>
    [XmlRoot(ElementName = "xml")]
    public class BaseMessage
    {
        /// <summary>
        /// 初始化一些内容，如创建时间为整形，
        /// </summary>
        public BaseMessage()
        {
            this.CreateTime = ConvertDateTimeInt(DateTime.Now);
        }

        private int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }  
        
        /// <summary>
        /// 开发者号
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public int CreateTime { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; }

        public virtual string ToXml()
        {
            this.CreateTime = (int)(DateTime.Now.Ticks);//重新更新
            return XmlHelper.Serializer(this);
        }
        public  static  BaseMessage GetInfo(string xml)
        {
            return XmlHelper.Deserializer<BaseMessage>(xml);
        }
    }
}