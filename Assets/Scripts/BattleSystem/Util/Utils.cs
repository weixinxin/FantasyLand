using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BattleSystem.Util
{
    public static class Utils
    {
        public static XmlDocument LoadXMLDocument(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }
            XmlDocument xml = new XmlDocument();
            XmlReaderSettings set = new XmlReaderSettings();
            set.IgnoreComments = true;//这个设置是忽略xml注释文档的影响。有时候注释会影响到xml的读取
            XmlReader reader = XmlReader.Create(fileName, set);
            if (reader == null)
            {
                return null;
            }
            xml.Load(reader);
            reader.Close();
            return xml;
        }

        public static string LoadFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return "";
            }
            string str = File.ReadAllText(fileName);
            return str;
        }
        public static T SerializeFromXml<T>(string filePath)
        {
            object result = null;

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    result = xmlSerializer.Deserialize(reader);
                }
            }

            return (T)result;
        }
        public static T SerializeFromString<T>(string text)
        {
            object result = null;

            using (StringReader reader = new StringReader(text))
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                result = xmlSerializer.Deserialize(reader);
            }

            return (T)result;
        }
        public static string SerializeToString(object sourceObj, Type type = null, Type[] extraTypes = null, string xmlRootName = null)
        {
            if (sourceObj != null)
            {
                type = type != null ? type : sourceObj.GetType();

                using (StringWriter writer = new StringWriter())
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer;
                    if (string.IsNullOrEmpty(xmlRootName))
                        xmlSerializer = extraTypes != null ? new System.Xml.Serialization.XmlSerializer(type, extraTypes) : new System.Xml.Serialization.XmlSerializer(type);
                    else
                        xmlSerializer = new System.Xml.Serialization.XmlSerializer(type, new XmlRootAttribute(xmlRootName));
                    xmlSerializer.Serialize(writer, sourceObj);

                    return writer.ToString();
                }
            }
            return null;
        }
    }

}
