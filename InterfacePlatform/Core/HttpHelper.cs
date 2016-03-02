using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;

namespace InterfacePlatform.Core
{
    public class HttpHelper
    {
        internal string GetHtml(string url)
        {
            try
            {
                WebClient wb = new WebClient();
                wb.Encoding = Encoding.UTF8;
                return wb.DownloadString(url);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}