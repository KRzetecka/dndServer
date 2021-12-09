using Polenter.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Server
{
    public class XmlTool
    {
        public static void SaveToFile<T>(T _object, string _filePath)
        {
            if (File.Exists(_filePath))
            {
                using (FileStream fs = File.OpenWrite(_filePath))
                {
                    var x = new XmlSerializer(_object.GetType());
                    x.Serialize(fs, _object);
                    fs.Close();
                }
            }
            else
            {
                using (FileStream fs = File.Create(_filePath))
                {
                    var x = new XmlSerializer(_object.GetType());
                    x.Serialize(fs, _object);
                    fs.Close();
                }
            }

        }
        public static void LoadFrom<T>(string _filePath, out T _object)
        {
            using (FileStream fs = File.OpenRead(_filePath))
            {
                var x = new XmlSerializer(typeof(T));
                
                _object = (T)x.Deserialize(fs);
                fs.Close();
            }
        }
        public static string SerializeObjectToXML<T>(T _object)
        {
            string msg;
            using (MemoryStream TextWriter = new MemoryStream())
            {
                XmlSerializer x = new XmlSerializer(typeof(T));
                
                x.Serialize(TextWriter, _object);
                msg = Encoding.ASCII.GetString(TextWriter.ToArray());               
            }
            return msg;
        }
        public static string SerializeStringToXML(string _text)
        {
            string msg;
            using (MemoryStream TextWriter = new MemoryStream())
            {
                XmlSerializer x = new XmlSerializer(typeof(string));
                x.Serialize(TextWriter, _text);
                msg = Encoding.ASCII.GetString(TextWriter.ToArray());
            }
            return msg;
        }

        public static T DeserializeXmlToObject<T>(string _msg)
        {
            T _object;
            byte[] byteArray = Encoding.ASCII.GetBytes(_msg);
            using (MemoryStream TextWriter = new MemoryStream(byteArray))
            {
                XmlSerializer x = new XmlSerializer(typeof(T));
                _object = (T)x.Deserialize(TextWriter);
            }
            return _object;
        }
        public static string DeserializeXmlToText(string _msg)
        {
            string result;
            byte[] byteArray = Encoding.ASCII.GetBytes(_msg);
            using (MemoryStream TextWriter = new MemoryStream(byteArray))
            {
                XmlSerializer x = new XmlSerializer(typeof(string));
                result = (string)x.Deserialize(TextWriter);
            }
            return result;
        }
        public static string SharpObjectToXMLString<T>(T _object)
        {
            string msg;
            using (MemoryStream TextWriter = new MemoryStream(10000))
            {
                SharpSerializerXmlSettings settings = new SharpSerializerXmlSettings();
                settings.Encoding = Encoding.ASCII;
                var x = new SharpSerializer(settings);
                x.Serialize(_object, TextWriter);
                msg = Encoding.ASCII.GetString(TextWriter.ToArray());
            }
            return msg;
        }
        public static T SharpXMLStringToObject<T>(string _data)
        {
            T _object;
            byte[] byteArray = Encoding.ASCII.GetBytes(_data);
            using (MemoryStream TextWriter = new MemoryStream(byteArray))
            {
                SharpSerializerXmlSettings settings = new SharpSerializerXmlSettings();
                settings.Encoding = Encoding.ASCII;
                settings.IncludeAssemblyVersionInTypeName = false;
                var x = new SharpSerializer(settings);
                _object = (T)x.Deserialize(TextWriter);
            }
            return _object;
        }
    }
}
