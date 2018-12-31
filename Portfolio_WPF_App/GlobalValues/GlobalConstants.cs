using Portfolio_WPF_App.Core.Handler;
using System.IO;

namespace Portfolio_WPF_App.GlobalValues
{
    public static class GlobalConstants
    {
        public static readonly string UI_USERNAME = "admin";
        public static readonly string UI_PASSWORD = "admin";

        public static readonly string LOG_FILE_FULL_PATH = Directory.GetCurrentDirectory() + "\\";
        public static readonly string LOG_FILE_NAME = "log.txt";

        public static readonly string CONFIG_FILE_PATH = Directory.GetCurrentDirectory() + "\\";
        public static readonly string CONFIG_FILE_NAME = "config.ini";

        public static readonly string STANDARD_XML_PATH = Directory.GetCurrentDirectory() + "\\";
        public static readonly string STANDARD_XML_NAME = "ExampleConfig.xml";

        public static LogLevel LOG_LEVEL = LogLevel.DEBUG;

        public static readonly string COMPANY_MAIL_ADDRESS = "tappler.andreas@gmail.com";
        public static readonly string COMPANY_WEB_ADDRESS = "https://github.com/JustForFunDeveloper/Portfolio_WPF_App";
    }
}
