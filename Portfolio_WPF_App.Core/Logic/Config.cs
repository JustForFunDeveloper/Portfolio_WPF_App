using System.Xml;
using System.Xml.Serialization;

namespace Portfolio_WPF_App.Core.Logic
{
    [XmlRoot("Portfolio_Config")]
    public class Config
    {
        [XmlAttribute]
        public string Description = "Configuration File";
        [XmlAttribute]
        public string Version = "1.0";
        public DBValues dBValues;
        public DebugLevel Debug_Level;


        public Config(string description, string version, DBValues dBValues, DebugLevel debug_Level)
        {
            this.Description = description;
            this.Version = version;
            this.dBValues = dBValues;
            this.Debug_Level = debug_Level;
        }

        public Config()
        {
            dBValues = new DBValues();
            Debug_Level = new DebugLevel();
        }
    }

    public class DBValues
    {
        [XmlAttribute]
        public string path = "";
        [XmlAttribute]
        public string name = "example.db";

        public DBValues(string path, string name)
        {
            this.path = path;
            this.name = name;
        }

        public DBValues()
        {
        }
    }

    public class DebugLevel
    {
        [XmlAttribute]
        public string level = "DEBUG";

        public DebugLevel(string level)
        {
            this.level = level;
        }

        public DebugLevel()
        {
        }
    }
}
