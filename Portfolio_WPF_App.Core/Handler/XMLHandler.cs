using Portfolio_WPF_App.Core.Logic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Portfolio_WPF_App.Core.Handler
{
    public class XMLHandler
    {
        public string GetSerializedConfigXML(Config config = null)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Config));
            using (var stringWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
                {
                    if (config == null)
                        LogHandler.WriteLog(this + ":  GetSerializedConfigXML -> config = null", LogLevel.ERROR);
                    //xmlSerializer.Serialize(writer, new Config());
                    else
                        xmlSerializer.Serialize(writer, config);
                    return stringWriter.ToString();
                }
            }
        }

        public Config GetDeserializedConfigObject(string xmlDocumentText)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            using (StringReader reader = new StringReader(xmlDocumentText))
            {
                return (Config)(serializer.Deserialize(reader));
            }
        }
    }
}
