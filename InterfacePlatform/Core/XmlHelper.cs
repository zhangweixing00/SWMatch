using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace InterfacePlatform.Core
{
    public class XmlHelper
    {
        public static T Deserializer<T>(string stringData)
           where T : class,new()//T必须有无参构造函数
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringReader sr = new StringReader(stringData))
                {
                    T dataInfo = serializer.Deserialize(sr) as T;
                    return dataInfo;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string Serializer<T>(T dataInfo)
           where T : class,new()//T必须有无参构造函数
        {
            try
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = false;
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (StringWriter sw = new StringWriter())
                {
                    serializer.Serialize(sw, dataInfo,ns);
                    string stringData = sw.ToString();
                    return stringData;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}