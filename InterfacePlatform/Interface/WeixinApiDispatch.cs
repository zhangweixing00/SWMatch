using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterfacePlatform.Core;
using InterfacePlatform.Log;

namespace InterfacePlatform.Interface
{
    public class WeixinApiDispatch
    {
        internal string Execute(string postStr)
        {
            BaseMessage baseMessage = BaseMessage.GetInfo(postStr);

            switch (baseMessage.MsgType)
            {
                 
                case "text":
                    LogUserOp.Log(baseMessage.FromUserName, string.Format("Query:{0}",postStr), 0);
                    MessageText message = MessageText.GetInfo(postStr);
                    string responseContent = BLLProcess.Entry(message.Content);
                    if (string.IsNullOrEmpty(responseContent))
                    {
                        responseContent = System.Configuration.ConfigurationManager.AppSettings["tip"];
                    }
                    return new ResponseText(message)
                    {
                        Content = responseContent

                    }.ToXml();

                case "event":
                    return ProcessEvent(postStr);
                default:
                    break;
            }

            return "";

        }

        private string ProcessEvent(string postStr)
        {
            MessageEvent me = MessageEvent.GetInfo(postStr);
            switch (me.Event)
            {
                case "subscribe":
                    return Subscribe(me);
                case "unsubscribe":
                    return UnSubscribe(me);
                case "CLICK":
                    return MenuClick(postStr);
                default:
                    break;
            }
            return "";
        }

        private string MenuClick(string postStr)
        {
            MessageEventClick me = MessageEventClick.GetInfo(postStr);
            switch (me.EventKey)
            {
                case "":
                    return new ResponseText()
                    {
                         Content=""
                    }.ToXml();
                default:
                    break;
            }
            return "";
        }

        private string UnSubscribe(MessageEvent me)
        {
            LogUserOp.Log(me.FromUserName, "UnSubscribe", 0);
            return "";
        }

        private string Subscribe(MessageEvent me)
        {
            LogUserOp.Log(me.FromUserName, "Subscribe", 0);
            string dy= System.Configuration.ConfigurationManager.AppSettings["dy"];
            return new ResponseText(me)
            {
                Content = dy

            }.ToXml();
        }
    }
}