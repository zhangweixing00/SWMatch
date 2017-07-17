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
                    LogUserOp.Log(baseMessage.FromUserName, string.Format("Query:{0}", postStr), 0);
                    MessageText message = MessageText.GetInfo(postStr);
                    return GetResponseTxt(baseMessage,message.Content);
                case "voice":
                    LogUserOp.Log(baseMessage.FromUserName, string.Format("Query:{0}", postStr), 1);
                    MessageVoice vMsg=MessageVoice.GetInfo(postStr);
                    return GetResponseTxt(baseMessage, vMsg.Recognition);
                case "event":
                    return ProcessEvent(postStr);
                default:
                    break;
            }

            return "";

        }

        private static string GetResponseTxt(BaseMessage baseMsg,string message)
        {
            string responseContent = BLLProcess.Entry(message);
            if (string.IsNullOrEmpty(responseContent))
            {
                responseContent = System.Configuration.ConfigurationManager.AppSettings["tip"];
            }
            return new ResponseText(baseMsg)
            {
                Content = responseContent

            }.ToXml();
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