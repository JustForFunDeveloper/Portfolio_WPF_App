using System;
using System.Text;

namespace Portfolio_WPF_App.Core.Handler
{
    [Flags]
    public enum LogLevel
    {
        DEBUG = 1,
        INFO = 2,
        WARNING = 3,
        ERROR = 4,
    }

    public class LogHandler
    {
        private static string _logFileName;
        private static string _logFilePath;
        private static FileHandler _fileHandler;
        private static LogLevel _logLevel;

        private static Int64 _debugCounter = 0;
        private static Int64 _warningCounter = 0;
        private static Int64 _infoCounter = 0;
        private static Int64 _errorCounter = 0;

        public static long DebugCounter { get => _debugCounter; }
        public static long WarningCounter { get => _warningCounter; }
        public static long InfoCounter { get => _infoCounter; }
        public static long ErrorCounter { get => _errorCounter; }
        public static LogLevel LogLevel { get => _logLevel; set => _logLevel = value; }

        public LogHandler(string logFileName, string logFilePath, LogLevel logLevel)
        {
            LogLevel = logLevel;
            _logFileName = logFileName;
            _logFilePath = logFilePath;
            _fileHandler = new FileHandler();
        }

        public static void WriteLog(string text, LogLevel logLevel)
        {
            if (logLevel >= LogLevel)
            {
                CreateLogFile();
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(DateTime.Now).Append(";").Append(logLevel.ToString()).Append(";").Append(text);
                _fileHandler.AppendText(GetCurrentLogName(), stringBuilder.ToString(), _logFilePath);

                switch (logLevel)
                {
                    case LogLevel.DEBUG:
                        _debugCounter++;
                        break;
                    case LogLevel.WARNING:
                        _warningCounter++;
                        break;
                    case LogLevel.INFO:
                        _infoCounter++;
                        break;
                    case LogLevel.ERROR:
                        _errorCounter++;
                        break;
                    default:
                        break;
                }
            }
        }

        private static void CreateLogFile()
        {
            _fileHandler.CreateFileIfNotExist(GetCurrentLogName(), _logFilePath);
        }

        public static string GetCurrentLogName()
        {
            DateTime date = DateTime.Now;
            string fileName = date.ToString("MMM.yyyy");
            fileName += "_" + _logFileName;
            return fileName;
        }

        public static string ReadLog()
        {
            return _fileHandler.ReadAllText(GetCurrentLogName(), _logFilePath);
        }
    }
}
