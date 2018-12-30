using Portfolio_WPF_App.GlobalValues;
using Portfolio_WPF_App.ViewModels.Handler;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Portfolio_WPF_App.ViewModels
{
    class LoginWindowModel : PropertyChangedViewModel
    {
        private ICommand _abort;
        private ICommand _ok;

        private Visibility _isErrorMsgVisible = Visibility.Hidden;

        private string _username = "";
        private string _default_Username = GlobalConstants.UI_USERNAME;
        private string _password = "";
        private string _default_Password = GlobalConstants.UI_PASSWORD;

        PasswordBox _passWordBox;

        public LoginWindowModel()
        {
            // PasswordBox Binding Workaround
            Mediator.Register("OnPasswordChanged", OnPassword);
            Mediator.Register("OnEnterPressed", OnEnterPressed);
        }

        public Visibility IsErrorMsgVisible
        {
            get { return _isErrorMsgVisible; }
            set
            {
                _isErrorMsgVisible = value;
                OnPropertyChanged();
            }
        }

        public void OnIsErrorMsgVisible(object value)
        {
            IsErrorMsgVisible = (Visibility)value;
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public void OnUsername(object value)
        {
            Username = (string)value;
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
            }
        }

        public void OnPassword(object value)
        {
            _passWordBox = (PasswordBox)value;
            Password = (string)_passWordBox.Password;

        }

        public ICommand Abort
        {
            get
            {
                if (_abort == null)
                {
                    _abort = new RelayCommand(
                        param => this.SaveAbortCommand(),
                        param => this.CanSaveAbortCommand()
                    );
                }
                return _abort;
            }
        }

        private bool CanSaveAbortCommand()
        {
            return true;
        }

        private void SaveAbortCommand()
        {
            Mediator.NotifyColleagues("CloseLoginWindow", true);
            Username = "";
            Password = "";
            if (_passWordBox != null)
                _passWordBox.Clear();
        }

        public ICommand Ok
        {
            get
            {
                if (_ok == null)
                {
                    _ok = new RelayCommand(
                        param => this.SaveOkCommand(),
                        param => this.CanSaveOkCommand()
                    );
                }
                return _ok;
            }
        }

        private bool CanSaveOkCommand()
        {
            return true;
        }

        private void SaveOkCommand()
        {
            if (Username.Equals(_default_Username) && Password.Equals(_default_Password))
            {
                Mediator.NotifyColleagues("OnLoginTry", true);
                IsErrorMsgVisible = Visibility.Hidden;
                Username = "";
                Password = "";
                if (_passWordBox != null)
                    _passWordBox.Clear();
            }
            else
            {
                IsErrorMsgVisible = Visibility.Visible;
            }
        }

        private void OnEnterPressed(object value)
        {
            SaveOkCommand();
        }
    }
}
