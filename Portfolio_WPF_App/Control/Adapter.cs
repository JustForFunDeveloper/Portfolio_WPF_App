using Portfolio_WPF_App.Core.Handler;
using Portfolio_WPF_App.Core.Logic;
using Portfolio_WPF_App.GlobalValues;
using Portfolio_WPF_App.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Portfolio_WPF_App.Control
{
    //TODO: Remove Test Methods
    public class Adapter
    {
        private Controller _controller = Controller.Instance;
        private LogicClass _logic;

        private int iter = 0;

        public Adapter()
        {
            _logic = new LogicClass(
                GlobalConstants.LOG_FILE_NAME,
                GlobalConstants.LOG_FILE_FULL_PATH,
                GlobalConstants.LOG_LEVEL);

            // Test method remove later
            _logic.TestEvent += OnTestEvent;

            #region Connected Home View Events
            _controller.ReloadHomeView += OnReloadHomeView;
            #endregion

            #region Connected Settings View Events
            _controller.ReloadSettingsView += OnReloadSettingsView;
            _controller.OpenConfig += OnOpenConfig;
            _controller.ActivateConfig += OnActivateConfig;
            #endregion

            #region Connected Data View Events
            _controller.RequestLogData += OnRequestLogData;
            _controller.RequestLogLevelFilteredData += OnRequestLogLevelFilteredData;
            _controller.SaveData += OnSaveData;
            #endregion

            LoadHomeView();
            LoadSettingsView();
            LoadDataView();
            LoadAboutView();
            LogHandler.WriteLog(this + " Created", LogLevel.DEBUG);
        }

        #region Home View
        private void OnReloadHomeView(object sender, object nothing)
        {
            LoadHomeView();
        }

        private void LoadHomeView()
        {
            _controller.TextError(LogHandler.ErrorCounter.ToString());
            _controller.TextWarning(LogHandler.WarningCounter.ToString());
            _controller.TextInfo(LogHandler.InfoCounter.ToString());
        }
        #endregion

        #region SettingsView
        private void OnReloadSettingsView(object sender, object nothing)
        {
            ReloadSettingsView();
        }

        private void ReloadSettingsView()
        {
            // Read the config file name but get all active settings from memory
            string configFileName = _logic.FileHandler.ReadAllText(GlobalConstants.CONFIG_FILE_NAME, GlobalConstants.CONFIG_FILE_PATH);
            _controller.TextConfigNameLoaded(configFileName);
            _controller.TextConfigLoaded(_logic.XmlHandler.GetSerializedConfigXML(_logic.Config));
            _controller.TextActiveConfigFile(configFileName);
        }

        private void LoadSettingsView()
        {
            // Read the config file name from the ini file and set all ui and config data from the current active file.
            string configFileName = _logic.FileHandler.ReadAllText(GlobalConstants.CONFIG_FILE_NAME, GlobalConstants.CONFIG_FILE_PATH);

            if (configFileName == null)
            {
                // Create new config.ini file since its missing and set it to standard values
                _logic.FileHandler.CreateFileIfNotExist(GlobalConstants.CONFIG_FILE_NAME, GlobalConstants.CONFIG_FILE_PATH, false);
                _logic.FileHandler.AppendText(GlobalConstants.CONFIG_FILE_NAME, GlobalConstants.STANDARD_XML_NAME, GlobalConstants.CONFIG_FILE_PATH, false);

                configFileName = GlobalConstants.STANDARD_XML_NAME;
                configFileName = configFileName.Replace(Environment.NewLine,"");
            }

            _controller.TextConfigNameLoaded(configFileName);

            string config = _logic.FileHandler.ReadAllText(configFileName, GlobalConstants.CONFIG_FILE_PATH);

            if (config == null)
            {
                // Create new xml file since its missing and set it to standard values
                string standardXML = _logic.XmlHandler.GetSerializedConfigXML(new Config());
                _logic.FileHandler.CreateFileIfNotExist(GlobalConstants.STANDARD_XML_NAME, GlobalConstants.STANDARD_XML_PATH, false);
                _logic.FileHandler.AppendText(GlobalConstants.STANDARD_XML_NAME, standardXML, GlobalConstants.STANDARD_XML_PATH);

                config = _logic.FileHandler.ReadAllText(configFileName, GlobalConstants.CONFIG_FILE_PATH);
            }

            _controller.TextConfigLoaded(config);
            _controller.TextActiveConfigFile(configFileName);
            _logic.Config = _logic.XmlHandler.GetDeserializedConfigObject(config);
        }

        private void OnOpenConfig(object sender, object list)
        {
            ListArguments listArgument = (ListArguments)list;
            List<object> data = (List<object>)listArgument.Value;
            _controller.TextConfigNameLoaded((string)data[1]);
            string path = (string)data[0];
            path = path.Replace((string)data[1], "");
            _controller.TextConfigLoaded(_logic.FileHandler.ReadAllText((string)data[1], path));
        }

        private void OnActivateConfig(object sender, object list)
        {
            ListArguments listArgument = (ListArguments)list;
            List<object> data = (List<object>)listArgument.Value;
            _controller.TextActiveConfigFile((string)data[0]);
            _logic.Config = _logic.XmlHandler.GetDeserializedConfigObject((string)data[1]);

            // Creates a local copy in the config file path if it doesn't exist.
            short result = _logic.FileHandler.CreateFileIfNotExist((string)data[0], GlobalConstants.CONFIG_FILE_PATH);
            if (result != 1 && result != -1)
                _logic.FileHandler.AppendText((string)data[0], (string)data[1], GlobalConstants.CONFIG_FILE_PATH);
            // Overwrite the config.ini with the current value.
            _logic.FileHandler.OverwriteFile(GlobalConstants.CONFIG_FILE_NAME, (string)data[0], GlobalConstants.CONFIG_FILE_PATH);
        }
        #endregion

        #region Data View
        private void OnRequestLogData(object sender, object nothing)
        {
            LoadDataView();
        }

        private void OnRequestLogLevelFilteredData(object sender, ListArguments e)
        {
            string data = LogHandler.ReadLog();
            string[] lines = data.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );
            List<object> list = new List<object>(lines);
            List<object> filteredList = new List<object>();

            int iter = 0;
            foreach (string item in list)
            {
                if (iter <= 1)
                {
                    filteredList.Add(item);
                    iter++;
                    continue;
                }
                foreach (LogLevel logLevel in e.Value)
                {
                    if (item.Contains(logLevel.ToString()))
                        filteredList.Add(item);
                }
            }
            _controller.LogData(new ListArguments(filteredList));
        }

        private void OnSaveData(object sender, object list)
        {
            ListArguments listArgument = (ListArguments)list;
            List<object> data = (List<object>)listArgument.Value;

            _logic.FileHandler.CreateFileIfNotExist((string)data[0]);
            string row = (string)data[1];
            string[] logList = row.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            _logic.FileHandler.AppendAll((string)data[0], logList);
        }

        private void LoadDataView()
        {
            string data = LogHandler.ReadLog();
            string[] lines = data.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );
            List<object> list = new List<object>(lines);
            _controller.LogData(new ListArguments(list));
        }
        #endregion

        #region About View
        private void LoadAboutView()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            _controller.TextVersion(version);
            _controller.TextLogicTitle(_logic.LogicTitle);
            _controller.TextLogicVersion(_logic.LogictVersion);
        }
        #endregion

        #region Test Method remove later
        private void OnTestEvent(object sender, BooleanArguments e)
        {
            _controller.DbConnected(!e.Value);

            _controller.TextActiveConfigFile("Connected " + e.Value + " with Test Event");
            iter++;
            _controller.TextMsgInDB(iter.ToString());
            iter++;
            _controller.TextError(iter.ToString());
            iter++;
            _controller.TextWarning(iter.ToString());
            iter++;
            _controller.TextInfo(iter.ToString());
            iter++;
            _controller.TextConfigNameLoaded(iter.ToString());
            iter++;
            _controller.TextConfigLoaded(iter.ToString());
            iter++;
        }
        #endregion
    }
}
