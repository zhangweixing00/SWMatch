using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Configuration;
using InterfacePlatform.Core;

namespace InterfacePlatform.Interface
{
    /// <summary>
    /// Entry 的摘要说明
    /// </summary>
    public class Entry : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string postString = "";
            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
            {
                using (Stream stream = HttpContext.Current.Request.InputStream)
                {
                    Byte[] postBytes = new Byte[stream.Length];
                    stream.Read(postBytes, 0, (Int32)stream.Length);
                    postString = Encoding.UTF8.GetString(postBytes);
                }

                if (!string.IsNullOrEmpty(postString))
                {
                    Execute(postString);
                }
            }
            else
            {
                Auth();
            }
        }
        /// <summary>
        /// 处理各种请求信息并应答（通过POST的请求）
        /// </summary>
        /// <param name="postStr">POST方式提交的数据</param>
        private void Execute(string postStr)
        {
            Log.Logger.logger.DebugFormat("Post:{0}", postStr);
            WeixinApiDispatch dispatch = new WeixinApiDispatch();
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            try
            {
                string responseContent = dispatch.Execute(postStr);
                //responseContent = "<?xml version=\"1.0\" encoding=\"utf-16\"?><xml><ToUserName>oSwljt2sD0LkLJ3y4SWyzj5m0bas</ToUserName><FromUserName>gh_b7697b14869d</FromUserName><CreateTime>15509374</CreateTime><MsgType>text</MsgType><Content>没有找到与gg相关的</Content></xml>";
                Log.Logger.logger.DebugFormat("ResponseContent:{0}", responseContent);
                HttpContext.Current.Response.Write(responseContent);
            }
            catch (Exception ex)
            {
                Log.Logger.logger.DebugFormat("error:{0}\r\n{1}", ex.Message, ex.StackTrace);
                HttpContext.Current.Response.Write("");
            }

        }

        /// <summary>
        /// 成为开发者的第一步，验证并相应服务器的数据
        /// </summary>
        private void Auth()
        {
            string token = ConfigurationManager.AppSettings["WeixinToken"];//从配置文件获取Token
            if (string.IsNullOrEmpty(token))
            {
                // LogTextHelper.Error(string.Format("WeixinToken 配置项没有配置！"));

            }
            Log.Logger.logger.DebugFormat("WeixinToken 配置项！{0}", token);
            string echoString = HttpContext.Current.Request.QueryString["echoStr"];
            string signature = HttpContext.Current.Request.QueryString["signature"];
            string timestamp = HttpContext.Current.Request.QueryString["timestamp"];
            string nonce = HttpContext.Current.Request.QueryString["nonce"];

            if (new BasicApi().CheckSignature(token, signature, timestamp, nonce))
            {
                if (!string.IsNullOrEmpty(echoString))
                {
                    Log.Logger.logger.DebugFormat("正确的验证:{0}", echoString);
                    HttpContext.Current.Response.Write(echoString);
                    HttpContext.Current.Response.End();
                }
                else
                {
                    Log.Logger.logger.Debug("无效的验证");
                }
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}