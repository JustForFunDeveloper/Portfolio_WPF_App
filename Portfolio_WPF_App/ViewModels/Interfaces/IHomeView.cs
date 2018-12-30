using System;

namespace Portfolio_WPF_App.ViewModels.Interfaces
{
    interface IHomeView
    {
        event EventHandler ReloadHomeView;
        void DbConnected(bool value);
        void TextMsgInDB(string value);
        void TextError(string value);
        void TextWarning(string value);
        void TextInfo(string value);
    }
}
