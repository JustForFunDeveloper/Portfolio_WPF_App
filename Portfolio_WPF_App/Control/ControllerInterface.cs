using System;
using System.Collections.Generic;

namespace Portfolio_WPF_App.Control
{
    public interface IController
    {
        #region HomeView
        event EventHandler ReloadHomeView;
        void DbConnected(bool value);  
        void TextMsgInDB(string value);
        void TextError(string value);
        void TextWarning(string value);
        void TextInfo(string value);
        #endregion

        #region SettingsView
        event EventHandler ReloadSettingsView;
        event EventHandler<ListArguments> OpenConfig;
        event EventHandler<ListArguments> ActivateConfig;
        void TextActiveConfigFile(string value);
        void TextConfigNameLoaded(string value);
        void TextConfigLoaded(string value);
        #endregion

        #region LogView
        event EventHandler ReloadDataView;
        event EventHandler<ListArguments> SaveData;
        void TextLogNameLoaded(string value);
        void TextLogLoaded(string value);
        #endregion

        #region AboutView
        void TextVersion(string value);
        void TextLogicTitle(string value);
        void TextLogicVersion(string value);
        #endregion
    }

    public class ListArguments : EventArgs
    {
        public List<string> Value;

        public ListArguments(List<string> value)
        {
            Value = value;
        }
    }
}
