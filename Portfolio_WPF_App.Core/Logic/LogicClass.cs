using Portfolio_WPF_App.Core.DataModel;
using Portfolio_WPF_App.Core.GlobalValues;
using Portfolio_WPF_App.Core.Handler;
using Portfolio_WPF_App.Core.Handler.DataBaseHandler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Portfolio_WPF_App.Core.Logic
{
    /// <summary>
    /// This class provides the logic to the UI.
    /// All important decision as well Handler like the DBHandler are located here.
    /// </summary>
    public class LogicClass
    {
        private string _logicVersion;
        private string _logicTitle;
        private FileHandler _fileHandler;
        private LogHandler _logHandler;
        private Config _config;
        private string _configFileName;
        private string _configAsString;
        private XMLHandler _xmlHandler;
        private static DBHandler dBHandler;
        private bool _statusDB = false;

        public string LogicVersion { get => _logicVersion; }
        public string LogicTitle { get => _logicTitle; }
        public LogHandler LogHandler { get => _logHandler; }
        public FileHandler FileHandler { get => _fileHandler; }
        public Config Config
        {
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
        public string ConfigFileName { get => _configFileName; set => _configFileName = value; }
        public string ConfigAsString { get => _configAsString; set => _configAsString = value; }
        public XMLHandler XmlHandler { get => _xmlHandler; set => _xmlHandler = value; }
        public bool StatusDB { get => _statusDB; set => _statusDB = value; }

        public event EventHandler DBConnected;
        public event EventHandler DBDisconnected;

        public LogicClass(string LogFileName, string LogFileFullPath, LogLevel logLevel)
        {
            _logHandler = new LogHandler(LogFileName, LogFileFullPath, logLevel);
            _fileHandler = new FileHandler();
            _xmlHandler = new XMLHandler();

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            _logicVersion = fileVersionInfo.FileVersion;
            _logicTitle = fileVersionInfo.FileDescription;

            LoadProfil();

            try
            {
                CreateDataBase();
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.ToString(), LogLevel.ERROR);
                return;
            }
            StatusDB = true;

            #region Hardcoded Test Values
            InsertTestDBValues();
            GenerateTestLogHandlerMessages();
            #endregion
        }

        #region Public Methods
        /// <summary>
        /// Loads the config file and saves it in the Config class.
        /// </summary>
        public void LoadProfil()
        {
            // Read the config file name from the ini file and set all ui and config data from the current active file.
            ConfigFileName = FileHandler.ReadAllText(LogicConstants.CONFIG_FILE_NAME, LogicConstants.CONFIG_FILE_PATH);

            if (ConfigFileName == null)
            {
                // Create new config.ini file since its missing and set it to standard values
                FileHandler.CreateFileIfNotExist(LogicConstants.CONFIG_FILE_NAME, LogicConstants.CONFIG_FILE_PATH, false);
                FileHandler.AppendText(LogicConstants.CONFIG_FILE_NAME, LogicConstants.STANDARD_XML_NAME, LogicConstants.CONFIG_FILE_PATH, false);

                ConfigFileName = LogicConstants.STANDARD_XML_NAME;
                ConfigFileName = ConfigFileName.Replace(Environment.NewLine, "");
            }

            ConfigAsString = FileHandler.ReadAllText(ConfigFileName, LogicConstants.CONFIG_FILE_PATH);

            if (ConfigAsString == null)
            {
                // Create new xml file since its missing and set it to standard values
                string standardXML = XmlHandler.GetSerializedConfigXML(new Config());
                FileHandler.CreateFileIfNotExist(LogicConstants.STANDARD_XML_NAME, LogicConstants.STANDARD_XML_PATH, false);
                FileHandler.AppendText(LogicConstants.STANDARD_XML_NAME, standardXML, LogicConstants.STANDARD_XML_PATH);

                ConfigAsString = FileHandler.ReadAllText(ConfigFileName, LogicConstants.CONFIG_FILE_PATH);
            }
            Config = XmlHandler.GetDeserializedConfigObject(ConfigAsString);
        }

        /// <summary>
        /// Fetches the current User and Password from the database and returns it.
        /// </summary>
        /// <returns>Returns the User and Password.</returns>
        public List<string> GetUserAndPassword()
        {
            List<List<object>> result = null;
            try
            {
                result = dBHandler.GetLastNRowsFromTable("UserAndPassword", 1);
                OnDBConnected();
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.ToString(), LogLevel.ERROR);
                OnDBDisconnected();
            }

            List<string> stringList = new List<string>() { (string)result[0][1], (string)result[0][2] };
            return stringList;
        }

        /// <summary>
        /// Saves the new Username into the database.
        /// </summary>
        /// <param name="Username">The Username to save.</param>
        public void SaveNewUserName(string Username)
        {
            List<Dictionary<string, Type>> rowsToUpdate = new List<Dictionary<string, Type>>()
                {
                    new Dictionary<string, Type>()
                        {
                            { "id" , typeof(int) },
                            { "User", typeof(string) }
                        }
                };

            List<List<object>> rowsData = new List<List<object>>()
                {
                    new List<object>()
                    {
                        1,
                        Username
                    }
                };
            try
            {
                dBHandler.UpdateTable("UserAndPassword", rowsToUpdate, rowsData);
                OnDBConnected();
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.ToString(), LogLevel.ERROR);
                OnDBDisconnected();
            }
        }

        /// <summary>
        /// Saves the new Password into the database.
        /// </summary>
        /// <param name="Password">The Password to save.</param>
        public void SaveNewPassword(string Password)
        {
            List<Dictionary<string, Type>> rowsToUpdate = new List<Dictionary<string, Type>>()
                {
                    new Dictionary<string, Type>()
                        {
                            { "id" , typeof(int) },
                            { "Password", typeof(string) }
                        }
                };

            List<List<object>> rowsData = new List<List<object>>()
                {
                    new List<object>()
                    {
                        1,
                        Password
                    }
                };
            try
            {
                dBHandler.UpdateTable("UserAndPassword", rowsToUpdate, rowsData);
                OnDBConnected();
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.ToString(), LogLevel.ERROR);
                OnDBDisconnected();
            }
        }

        /// <summary>
        /// Fetches all Data from the table and returns it.
        /// </summary>
        /// <returns>Returns all data.</returns>
        public List<object> GetAllDbData()
        {
            List<List<object>> result = null;
            try
            {
                result = dBHandler.GetLastNRowsFromTable("Data", 1000);
                OnDBConnected();
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.ToString(), LogLevel.ERROR);
                OnDBDisconnected();
            }

            List<object> objectList = new List<object>();
            if (result == null)
                return null;
            else
            {
                foreach (List<object> row in result)
                {
                    objectList.Add(new Data()
                    {
                        Id = (int)row[0],
                        Time = (string)row[1],
                        Name = (string)row[2],
                        SureName = (string)row[3],
                        Message = (string)row[4]
                    });
                }
            }
            return objectList;
        }

        /// <summary>
        /// Gets the current rows number from the data table.
        /// </summary>
        /// <returns>The number of rows in the table.</returns>
        public int GetCurrentRowsFromData()
        {
            int result = 0;
            try
            {
                result = dBHandler.GetCurrentRowsFromTable("Data");
                OnDBConnected();
            }
            catch (Exception e)
            {
                LogHandler.WriteLog(e.ToString(), LogLevel.ERROR);
                OnDBDisconnected();
            }
            return result;
        }
        #endregion

        #region Private Methods
        private void CreateNewLogHandler(string LogFileName, string LogFileFullPath, LogLevel logLevel)
        {
            _logHandler = new LogHandler(LogFileName, LogFileFullPath, logLevel);
        }

        private void CreateDataBase()
        {
            dBHandler = new DBHandler(
                Config.dBValues.name,
                Config.dBValues.path,
                CreateTables(),
                DataBaseType.SQLITE);
            InsertStandardPasswordIfNotExist();
        }

        private List<Table> CreateTables()
        {
            // TODO: Delete this hardcoded code and get this data Dictionary from the DataModel!
            List<Table> dataTable = new List<Table>
            {
                new Table(
                "Data",
                new Dictionary<string, Type>()
                {
                    { "Id" , typeof(int) },
                    { "Time", typeof(string) },
                    { "Name", typeof(string) },
                    { "SureName", typeof(string) },
                    { "Message", typeof(string) }
                },
                100),

                new Table(
                "UserAndPassword",
                new Dictionary<string, Type>()
                {
                    { "Id" , typeof(int) },
                    { "User", typeof(string) },
                    { "Password", typeof(string) }
                },
                100)
            };

            return dataTable;
        }

        private void InsertStandardPasswordIfNotExist()
        {
            if (dBHandler.GetCurrentRowsFromTable("UserAndPassword") < 1)
            {
                dBHandler.InsertIntoTable("UserAndPassword", new List<List<object>>()
                {
                    new List<object>()
                    {
                        0,
                        "admin",
                        "admin"
                    }
                });
            }
        }

        private void InsertTestDBValues()
        {
            if (dBHandler.GetCurrentRowsFromTable("Data") < 1)
                dBHandler.InsertIntoTable("Data", DBEntryGenerator.GetDbRows(20));
        }

        private void GenerateTestLogHandlerMessages()
        {
            LogHandler.WriteLog(this + ": DummyMessage 1", LogLevel.ERROR);
            LogHandler.WriteLog(this + ": DummyMessage 2", LogLevel.ERROR);
            LogHandler.WriteLog(this + ": DummyMessage 3", LogLevel.ERROR);
            LogHandler.WriteLog(this + ": DummyMessage 4", LogLevel.INFO);
            LogHandler.WriteLog(this + ": DummyMessage 5", LogLevel.INFO);
            LogHandler.WriteLog(this + ": DummyMessage 6", LogLevel.WARNING);
        }
        #endregion

        #region Logic EventMethods
        protected virtual void OnDBConnected()
        {
            DBConnected?.Invoke(this, null);
        }

        protected virtual void OnDBDisconnected()
        {
            DBDisconnected?.Invoke(this, null);
        }
        #endregion
    }
}
