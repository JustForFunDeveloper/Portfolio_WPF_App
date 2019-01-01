using System;

namespace Portfolio_WPF_App.ViewModels.Interfaces
{
    interface ILoginWindow
    {
        event EventHandler GetLoginData;
        void LoginData(string User, string Password);
    }
}
