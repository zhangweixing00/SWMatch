using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace InterfacePlatform
{
    public class Global : System.Web.HttpApplication
    {
        public static List<SW.MiddlePlugin.IMiddlePlugin> MPList { get; set; }
        void Application_Start(object sender, EventArgs e)
        {
            //在应用程序启动时运行的代码
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(path));
            InitMpList();
            Log.Logger.logger.DebugFormat("共加载中间件：{0}个", MPList.Count);

        }

        private void InitMpList()
        {
            MPList = new List<SW.MiddlePlugin.IMiddlePlugin>();
            string libPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mp");
            Log.Logger.logger.DebugFormat("中间件路径：{0}", libPath);
            if (Directory.Exists(libPath))
            {
                string[] libs = Directory.GetFiles(libPath, "*.dll", SearchOption.TopDirectoryOnly);
                foreach (var item in libs)
                {
                    Assembly assembly = Assembly.LoadFile(item);
                    Type[] types = assembly.GetTypes();
                    foreach (var t in types)
                    {
                        if (t.GetInterface("SW.MiddlePlugin.IMiddlePlugin") != null)
                        {
                            var instance = (SW.MiddlePlugin.IMiddlePlugin)Activator.CreateInstance(t);
                            Log.Logger.logger.DebugFormat("加载中间件：{0}", t.Name);
                            MPList.Add(instance);
                        }
                    }
                }
            }
        }

        void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码

        }

        void Application_Error(object sender, EventArgs e)
        {
            // 在出现未处理的错误时运行的代码

        }

        void Session_Start(object sender, EventArgs e)
        {
            // 在新会话启动时运行的代码

        }

        void Session_End(object sender, EventArgs e)
        {
            // 在会话结束时运行的代码。 
            // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
            // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer 
            // 或 SQLServer，则不会引发该事件。

        }

    }
}
