using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace InterfacePlatform.Core
{
    public class BasicApi
    {
        /// <summary>
        /// 验证企业号签名
        /// </summary>
        /// <param name="token">企业号配置的Token</param>
        /// <param name="signature">签名内容</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">nonce参数</param>
        /// <param name="corpId">企业号ID标识</param>
        /// <param name="encodingAESKey">加密键</param>
        /// <param name="echostr">内容字符串</param>
        /// <param name="retEchostr">返回的字符串</param>
        /// <returns></returns>
        public bool CheckSignature(string token, string signature, string timestamp, string nonce)
        {
            string[] ArrTmp = { token, timestamp, nonce };

            Array.Sort(ArrTmp);
            string tmpStr = string.Join("", ArrTmp);

            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            tmpStr = tmpStr.ToLower();

            if (tmpStr == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}