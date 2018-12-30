using Portfolio_WPF_App.ViewModels.Handler;
using Portfolio_WPF_App.ViewModels.Interfaces;
using System;
using System.Collections.Generic;

namespace Portfolio_WPF_App.Control
{
    public class Controller : IHomeView
    {
        private static Controller instance = null;
        private static readonly object padlock = new object();

        #region Home View Events
        /// <summary>
        /// <see cref="ViewModels.HomeViewModel"/>
        /// Is invoked if the ReloadHomeView signal is sent.
        /// </summary>
        public event EventHandler ReloadHomeView;
        #endregion

        // TODO: Save -NewUsername and -NewPassword Event is missing.
        #region Settings View Events
        /// <summary>
        /// <see cref="ViewModels.SettingsViewModel"/>
        /// Is invoked if the ReloadSettingsView signal is sent.
        /// </summary>
        public event EventHandler ReloadSettingsView;
        /// <summary>
        /// <see cref="ViewModels.SettingsViewModel"/><para />
        /// Is invoked when the Open Config button is pressed.
        /// </summary>
        public event EventHandler<ListArguments> OpenConfig;
        /// <summary>
        /// <see cref="ViewModels.SettingsViewModel"/><para />
        /// Is invoked when the Save Config button is pressed.
        /// </summary>
        public event EventHandler<ListArguments> ActivateConfig;
        #endregion

        #region Data View Events
        /// <summary>
        /// <see cref="ViewModels.DataViewModel"/>
        /// Is invoked if the ReloadDataView signal is sent.
        /// </summary>
        public event EventHandler ReloadDataView;
        /// <summary>
        /// <see cref="ViewModels.DataViewModel"/>
        /// Is invoked if the Save Data button is pressed.
        /// </summary>
        public event EventHandler<ListArguments> SaveData;
        #endregion

        public Controller()
        {
            Mediator.Register("ReloadHomeView", OnReloadHomeView);

            Mediator.Register("ReloadSettingsView", OnReloadSettingsView);
            Mediator.Register("OpenConfigCommand", OnOpenConfig);
            Mediator.Register("ActivateConfigCommand", OnActivateConfig);

            Mediator.Register("ReloadDataView", OnReloadDataView);
            Mediator.Register("SaveLogCommand", OnSaveLog);
        }

        /// <summary>
        /// Singleton implementation. Thread safe through locking.
        /// </summary>
        public static Controller Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Controller();
                    }
                    return instance;
                }
            }
        }

        #region Home View
        /// <summary>
        /// <see cref="ViewModels.HomeViewModel"/><para />
        /// Shows the correct Message as while icon if the database is connected.
        /// </summary>
        /// <param name="value">true if it is connected.
        ///                     false if it isn't connected.</param>
        public void DbConnected(bool value)
        {
            Mediator.NotifyColleagues("OnDbConnected", value);
        }

        /// <summary>
        /// <see cref="ViewModels.HomeViewModel"/><para />
        /// Displays the current number of messages in the database.
        /// </summary>
        /// <param name="value">string value of the messages in the database.</param>
        public void TextMsgInDB(string value)
        {
            //TODO: Rename this
            Mediator.NotifyColleagues("OnTextMsgInDB", value);
        }

        /// <summary>
        /// <see cref="ViewModels.HomeViewModel"/><para />
        /// Displays the current number of messages with the value error.
        /// </summary>
        /// <param name="value">string value of the error messages.</param>
        public void TextError(string value)
        {
            Mediator.NotifyColleagues("OnTextError", value);
        }

        /// <summary>
        /// <see cref="ViewModels.HomeViewModel"/><para />
        /// Displays the current number of messages with the value warning.
        /// </summary>
        /// <param name="value">string value of the warning messages.</param>
        public void TextWarning(string value)
        {
            Mediator.NotifyColleagues("OnTextWarning", value);
        }

        /// <summary>
        /// <see cref="ViewModels.HomeViewModel"/><para />
        /// Displays the current number of messages with the value info.
        /// </summary>
        /// <param name="value">string value of the info messages.</param>
        public void TextInfo(string value)
        {
            Mediator.NotifyColleagues("OnTextInfo", value);
        }
        #endregion

        #region Settings View
        /// <summary>
        /// <see cref="ViewModels.SettingsViewModel"/><para />
        /// Displays the current activ config file name.
        /// </summary>
        /// <param name="value">string of the current active config file name.</param>
        public void TextActiveConfigFile(string value)
        {
            Mediator.NotifyColleagues("OnTextActiveConfigFile", value);
        }

        /// <summary>
        /// <see cref="ViewModels.SettingsViewModel"/><para />
        /// Displays the current loaded config file name.
        /// </summary>
        /// <param name="value">string value of the loaded config file name.</param>
        public void TextConfigNameLoaded(string value)
        {
            Mediator.NotifyColleagues("OnTextConfigNameLoaded", value);
        }

        /// <summary>
        /// <see cref="ViewModels.SettingsViewModel"/><para />
        /// Displays the current loaded config.
        /// </summary>
        /// <param name="value">string value of the loaded config.</param>
        public void TextConfigLoaded(string value)
        {
            Mediator.NotifyColleagues("OnTextConfigLoaded", value);
        }
        #endregion

        #region Data View
        /// <summary>
        /// <see cref="ViewModels.DataViewModel"/><para />
        /// Displays the current loaded log file name.
        /// </summary>
        /// <param name="value">string value of the loaded config file name.</param>
        public void TextLogNameLoaded(string value)
        {
            Mediator.NotifyColleagues("OnTextLogFileName", value);
        }

        /// <summary>
        /// <see cref="ViewModels.DataViewModel"/><para />
        /// Displays the current loaded log.
        /// </summary>
        /// <param name="value">string value of the loaded config.</param>
        public void TextLogLoaded(string value)
        {
            Mediator.NotifyColleagues("OnTextLogFile", value);
        }
        #endregion

        #region About View
        /// <summary>
        /// <see cref="ViewModels.AboutViewModel"/><para />
        /// Displays the current ui version number.
        /// </summary>
        /// <param name="value">string value of the ui version number.</param>
        public void TextVersion(string value)
        {
            Mediator.NotifyColleagues("OnTextVersion", value);
        }

        /// <summary>
        /// <see cref="ViewModels.AboutViewModel"/><para />
        /// Displays the current log title.
        /// </summary>
        /// <param name="value">string value of the logic title.</param>
        public void TextLogicTitle(string value)
        {
            Mediator.NotifyColleagues("OnTextLogicTitle", value);
        }

        /// <summary>
        /// <see cref="ViewModels.AboutViewModel"/><para />
        /// Displays the current logic version number.
        /// </summary>
        /// <param name="value">string value of the logic version number.</param>
        public void TextLogicVersion(string value)
        {
            Mediator.NotifyColleagues("OnTextLogicVersion", value);
        }
        #endregion

        #region Event Invoke Methods
        /// <summary>
        /// Catches the Mediator message and invokes the <see cref="ReloadHomeView"/> event.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnReloadHomeView(object value)
        {
            ReloadHomeView?.Invoke(this, null);
        }

        /// <summary>
        /// Catches the Mediator message and invokes the <see cref="OnReloadSettingsView"/> event.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnReloadSettingsView(object value)
        {
            ReloadSettingsView?.Invoke(this, null);
        }

        /// <summary>
        /// Catches the Mediator message and invokes the <see cref="OpenConfig"/> event.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnOpenConfig(object value)
        {
            List<object> list = (List<object>)value;
            OpenConfig?.Invoke(this, new ListArguments(list));
        }

        /// <summary>
        /// Catches the Mediator message and invokes the <see cref="ActivateConfig"/> event.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnActivateConfig(object value)
        {
            List<object> list = (List<object>)value;
            ActivateConfig?.Invoke(this, new ListArguments(list));
        }

        /// <summary>
        /// Catches the Mediator message and invokes the <see cref="OnReloadDataView"/> event.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnReloadDataView(object value)
        {
            ReloadDataView?.Invoke(this, null);
        }

        /// <summary>
        /// Catches the Mediator message and invokes the <see cref="SaveData"/> event.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnSaveLog(object value)
        {
            List<object> list = (List<object>)value;
            SaveData?.Invoke(this, new ListArguments(list));
        }
        #endregion
    }
}
