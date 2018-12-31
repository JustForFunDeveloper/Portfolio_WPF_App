using Portfolio_WPF_App.Core.Handler;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Portfolio_WPF_App.Core.Logic
{
    public class LogicClass
    {
        #region Test Variables remove later
        public event EventHandler<BooleanArguments> TestEvent;
        public Thread thread;
        public bool _closeThread = true;
        #endregion

        private string _logicVersion;
        private string _logicTitle;
        private FileHandler _fileHandler;
        private LogHandler _logHandler;
        private Config _config;
        private XMLHandler _xmlHandler;
        //private static DBHandler dBHandler;

        public string LogictVersion { get => _logicVersion; }
        public string LogicTitle { get => _logicTitle; }
        public LogHandler LogHandler { get => _logHandler; }
        public FileHandler FileHandler { get => _fileHandler; }
        public Config Config {
            get => _config;
            set
            {
                _config = value;
                if (Enum.TryParse(Config.Debug_Level.level, out LogLevel loglevel))
                    LogHandler.LogLevel = loglevel;
                else
                    LogHandler.WriteLog("Couldn't change loglevel of the LogHandler", LogLevel.ERROR);

            }
        }
        public XMLHandler XmlHandler { get => _xmlHandler; set => _xmlHandler = value; }

        public LogicClass(string LogFileName, string LogFileFullPath, LogLevel logLevel)
        {
            _logHandler = new LogHandler(LogFileName, LogFileFullPath, logLevel);
            _fileHandler = new FileHandler();
            _xmlHandler = new XMLHandler();

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            _logicVersion = fileVersionInfo.FileVersion;
            _logicTitle = fileVersionInfo.FileDescription;
            CreateDataBase();
        }

        private void CreateNewLogHandler(string LogFileName, string LogFileFullPath, LogLevel logLevel)
        {
            _logHandler = new LogHandler(LogFileName, LogFileFullPath, logLevel);
        }

        private void CreateDataBase()
        {
            //dBHandler = new DBHandler("Data Source = " + GlobalConstants.DB_RELATIVE_PATH + GlobalConstants.DB_NAME + "; Version = 3;");
            //dBHandler.CommitQuery(DataBaseQueries.CreateDBQuery);
        }

        // TestMethods .... remove later
        #region Test
        public void PrintTestXML(Config config)
        {
            FileHandler xmlFileHandler = new FileHandler();
            string text = _xmlHandler.GetSerializedConfigXML(config);
            if (xmlFileHandler.CreateFileIfNotExist("test.xml", "", false) == 1)
                xmlFileHandler.OverwriteFile("test.xml", text, "");
            else
                xmlFileHandler.AppendText("test.xml", text);
        }

        public void RaiseEventAfterTimeOut(int timeOut)
        {
            bool value = true;
            thread = new Thread(() =>
            {
                while (_closeThread)
                {
                    Thread.Sleep(timeOut);
                    OnTestEvent(new BooleanArguments(value));
                    value = !value;
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        protected virtual void OnTestEvent(BooleanArguments e)
        {
            TestEvent?.Invoke(this, e);
        }

        public void EndThread()
        {
            _closeThread = false;
        }
        #endregion
    }

    public class BooleanArguments : EventArgs
    {
        public bool Value;

        public BooleanArguments(bool value)
        {
            Value = value;
        }
    }
}
