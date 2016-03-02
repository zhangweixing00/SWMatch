<%@ WebHandler Language="C#" Class="zz" %>
using System;
using System.Web;
public class zz : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        using (System.IO.StreamReader sr = new System.IO.StreamReader(context.Request.InputStream))
        {
            context.Response.ContentType = "text/plain";
            string line = sr.ReadToEnd();
            System.Net.WebClient wb = new System.Net.WebClient();
            wb.Encoding = System.Text.Encoding.UTF8;
            string rs = wb.UploadString("http://localhost:53023/Interface/Entry.ashx", "POST", line);
            context.Response.Write(rs);
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


