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
    public class Adapter
    {
        private Controller _controller = Controller.Instance;
        private LogicClass _logic;

        private int iter = 0;

        public Adapter()
        {
            // TODO:Change complete Logic!

            _logic = new LogicClass(
                GlobalConstants.LOG_FILE_NAME,
                GlobalConstants.LOG_FILE_FULL_PATH,
                GlobalConstants.LOG_LEVEL);

            // Test method remove later
            _logic.TestEvent += OnTestEvent;

            _controller.ReloadHomeView += OnReloadHomeView;
            _controller.ReloadSettingsView += OnReloadSettingsView;
            _controller.OpenConfig += OnOpenConfig;
            _controller.ActivateConfig += OnActivateConfig;
            _controller.ReloadDataView += OnReloadDataView;
            _controller.SaveData += OnSaveData;

            #region AboutView startup values 
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            _controller.TextVersion(version);
            _controller.TextLogicTitle(_logic.LogicTitle);
            _controller.TextLogicVersion(_logic.LogictVersion);
            #endregion

            ReloadDataView();
            StartUpSettingsView();
        }

        public void OnReloadHomeView(object sender, object nothing)
        {
            LoadHomeViewSettings();
        }

        private void LoadHomeViewSettings()
        {
            #region Messages & Report
            // TODO: Implement Message Buffer and Message Sent Counter
            _controller.TextError(LogHandler.ErrorCounter.ToString());
            _controller.TextWarning(LogHandler.WarningCounter.ToString());
            _controller.TextInfo(LogHandler.InfoCounter.ToString());
            #endregion
        }

        public void OnReloadSettingsView(object sender, object nothing)
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

        private void StartUpSettingsView()
        {
            Console.WriteLine("StartUpSettingsView");
            // Read the config file name from the ini file and set all ui and config data from the current active file.
            string configFileName = _logic.FileHandler.ReadAllText(GlobalConstants.CONFIG_FILE_NAME, GlobalConstants.CONFIG_FILE_PATH);
            _controller.TextConfigNameLoaded(configFileName);

            // Check null, if file cant be found create a new empty config file and new standard config!
            string config = _logic.FileHandler.ReadAllText(configFileName, GlobalConstants.CONFIG_FILE_PATH);

            if (config == null)
                return;

            _controller.TextConfigLoaded(config);
            _controller.TextActiveConfigFile(configFileName);
            _logic.Config = _logic.XmlHandler.GetDeserializedConfigObject(config);
        }

        public void OnOpenConfig(object sender, object list)
        {
            ListArguments listArgument = (ListArguments)list;
            List<object> data = (List<object>)listArgument.Value;
            _controller.TextConfigNameLoaded((string)data[1]);
            string path = (string)data[0];
            path = path.Replace((string)data[1], "");
            _controller.TextConfigLoaded(_logic.FileHandler.ReadAllText((string)data[1], path));
        }

        public void OnActivateConfig(object sender, object list)
        {
            ListArguments listArgument = (ListArguments)list;
            List<object> data = (List<object>)listArgument.Value;
            _controller.TextActiveConfigFile((string)data[0]);
            _logic.Config = _logic.XmlHandler.GetDeserializedConfigObject((string)data[1]);
            //_logic.PrintTestXML(_logic.Config);

            // Creates a local copy in the config file path if it doesn't exist.
            short result = _logic.FileHandler.CreateFileIfNotExist((string)data[0], GlobalConstants.CONFIG_FILE_PATH);
            if (result != 1 && result != -1)
                _logic.FileHandler.AppendText((string)data[0], (string)data[1], GlobalConstants.CONFIG_FILE_PATH);
            // Overwrite the config.ini with the current value.
            _logic.FileHandler.OverwriteFile(GlobalConstants.CONFIG_FILE_NAME, (string)data[0], GlobalConstants.CONFIG_FILE_PATH);
        }

        public void OnReloadDataView(object sender, object nothing)
        {
            ReloadDataView();
        }

        private void ReloadDataView()
        {
            _controller.TextLogNameLoaded(LogHandler.GetCurrentLogName());
            _controller.TextLogLoaded(LogHandler.ReadLog());
        }

        public void OnSaveData(object sender, object list)
        {
            ListArguments listArgument = (ListArguments)list;
            List<object> data = (List<object>)listArgument.Value;

            _logic.FileHandler.CreateFileIfNotExist((string)data[0]);
            string row = (string)data[1];
            string[] logList = row.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            _logic.FileHandler.AppendAll((string)data[0], logList);
        }

        #region Test Method remove later
        public void OnTestEvent(object sender, BooleanArguments e)
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
            _controller.TextLogNameLoaded(iter.ToString());
            iter++;
            _controller.TextLogLoaded(iter.ToString());
            iter++;
        }
        #endregion
    }
}
