using System;

namespace Portfolio_WPF_App.ViewModels.Interfaces
{
    interface ISettingsView
    {
        event EventHandler ReloadSettingsView;
        event EventHandler<ListArguments> OpenConfig;
        event EventHandler<ListArguments> ActivateConfig;
        event EventHandler<ListArguments> SaveNewUserName;
        event EventHandler<ListArguments> SaveNewPassword;
        void TextConfigNameLoaded(string value);
        void TextConfigLoaded(string value);
        void TextActiveConfigFile(string value);
    }
}
