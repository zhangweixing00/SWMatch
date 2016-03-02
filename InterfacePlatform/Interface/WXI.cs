using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterfacePlatform.Core;

namespace InterfacePlatform.Interface
{
    public static class WXI
    {
        public static string Anaysle()
        {
            return "";
        }

        public static string ToResponse(this string reponseTxt)
        {
            var tempEntity = new { ID = 0, Name = string.Empty };
            string json = JsonHelper.SerializeObject(tempEntity);
            return json;
        }
    }
}