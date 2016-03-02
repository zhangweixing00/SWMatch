using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterfacePlatform.Core;
using System.Text.RegularExpressions;

namespace InterfacePlatform.Interface.Users
{
    public class AccessToken
    {
        /// <summary>
        /// 获取每次操作微信API的Token访问令牌
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="secret">开发者凭据</param>
        /// <returns></returns>
        public string GetAccessToken(string appid, string secret)
        {
            //正常情况下access_token有效期为7200秒,这里使用缓存设置短于这个时间即可
            string access_token = MemoryCacheHelper.GetCacheItem<string>("access_token", delegate()
            {
                string grant_type = "client_credential";
                var url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type={0}&appid={1}&secret={2}",
                                        grant_type, appid, secret);

                HttpHelper helper = new HttpHelper();
                string result = helper.GetHtml(url);
                string regex = "\"access_token\":\"(?<token>.*?)\"";
                string token = RegexHelper.GetText(result, regex, "token");
                return token;
            },
                new TimeSpan(0, 0, 7000)//7000秒过期
            );

            return access_token;
        }
    }
}