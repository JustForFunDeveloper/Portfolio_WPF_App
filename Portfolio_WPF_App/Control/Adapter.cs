using Portfolio_WPF_App.Core.GlobalValues;
using Portfolio_WPF_App.Core.Handler;
using Portfolio_WPF_App.Core.Logic;
using Portfolio_WPF_App.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Portfolio_WPF_App.Control
{
    /// <summary>
    /// This class connects the UI's Controller wit the Logic.
    /// </summary>
    public class Adapter
    {
        private Controller _controller = Controller.Instance;
        private LogicClass _logic;

        public Adapter()
        {
            _logic = new LogicClass(
                LogicConstants.LOG_FILE_NAME,
                LogicConstants.LOG_FILE_FULL_PATH,
                LogicConstants.LOG_LEVEL);

            #region Connected Logic Events
            _logic.DBConnected += OnDBConnected;
            _logic.DBDisconnected += OnDBDisconnected;
            #endregion

            #region Connected Home View Events
            _controller.ReloadHomeView += OnReloadHomeView;
            #endregion

            #region Connected Settings View Events
            _controller.ReloadSettingsView += OnReloadSettingsView;
            _controller.OpenConfig += OnOpenConfig;
            _controller.ActivateConfig += OnActivateConfig;
            _controller.SaveNewUserName += OnSaveNewUserName;
            _controller.SaveNewPassword += OnSaveNewPassword;
            #endregion

            #region Connected Data View Events
            _controller.RequestLogData += OnRequestLogData;
            _controller.RequestLogLevelFilteredData += OnRequestLogLevelFilteredData;
            _controller.SaveData += OnSaveData;
            #endregion

            #region Connected Login Window Events
            _controller.GetLoginData += OnGetLoginData;
            #endregion

            LoadHomeView();
            LoadSettingsView();
            LoadDataView();
            LoadAboutView();
            LogHandler.WriteLog(this + " Created", LogLevel.DEBUG);
        }

        #region Logic
        private void OnDBConnected(object sender, EventArgs e)
        {
            _controller.DbConnected(true);
            Console.WriteLine(this + "DbConnected");
        }

        private void OnDBDisconnected(object sender, EventArgs e)
        {
            _controller.DbConnected(false);
        }
        #endregion

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
            _controller.DbConnected(_logic.StatusDB);
            _controller.TextMsgInDB(_logic.GetCurrentRowsFromData().ToString());
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
            string configFileName = _logic.FileHandler.ReadAllText(LogicConstants.CONFIG_FILE_NAME, LogicConstants.CONFIG_FILE_PATH);
            _controller.TextConfigNameLoaded(configFileName);
            _controller.TextConfigLoaded(_logic.XmlHandler.GetSerializedConfigXML(_logic.Config));
            _controller.TextActiveConfigFile(configFileName);
        }

        private void LoadSettingsView()
        {
            _logic.LoadProfil();
            _controller.TextConfigNameLoaded(_logic.ConfigFileName);
            _controller.TextConfigLoaded(_logic.ConfigAsString);
            _controller.TextActiveConfigFile(_logic.ConfigFileName);
        }

        private void OnOpenConfig(object sender, ListArguments list)
        {
            List<object> data = list.Value;
            _controller.TextConfigNameLoaded((string)data[1]);
            string path = (string)data[0];
            path = path.Replace((string)data[1], "");
            _controller.TextConfigLoaded(_logic.FileHandler.ReadAllText((string)data[1], path));
        }

        private void OnActivateConfig(object sender, ListArguments list)
        {
            List<object> data = list.Value;
            _controller.TextActiveConfigFile((string)data[0]);
            _logic.Config = _logic.XmlHandler.GetDeserializedConfigObject((string)data[1]);

            // Creates a local copy in the config file path if it doesn't exist.
            short result = _logic.FileHandler.CreateFileIfNotExist((string)data[0], LogicConstants.CONFIG_FILE_PATH);
            if (result != 1 && result != -1)
                _logic.FileHandler.AppendText((string)data[0], (string)data[1], LogicConstants.CONFIG_FILE_PATH);
            // Overwrite the config.ini with the current value.
            _logic.FileHandler.OverwriteFile(LogicConstants.CONFIG_FILE_NAME, (string)data[0], LogicConstants.CONFIG_FILE_PATH);
        }

        private void OnSaveNewUserName(object sender, ListArguments list)
        {
            List<object> data = list.Value;
            _logic.SaveNewUserName((string)data[0]);
        }

        private void OnSaveNewPassword(object sender, ListArguments list)
        {
            List<object> data = list.Value;
            _logic.SaveNewPassword((string)data[0]);
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
            if (data == null)
                return;

            string[] lines = data.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );
            List<object> list = new List<object>(lines);
            _controller.LogData(new ListArguments(list));

            _controller.Data(new ListArguments(_logic.GetAllDbData()));
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
            _controller.TextLogicVersion(_logic.LogicVersion);
        }
        #endregion

        #region Login Window
        private void OnGetLoginData(object sender, object nothing)
        {
            List<string> resultList = _logic.GetUserAndPassword();
            _controller.LoginData(resultList[0], resultList[1]);
        }
        #endregion
    }
}
