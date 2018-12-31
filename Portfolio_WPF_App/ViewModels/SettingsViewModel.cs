using Microsoft.Win32;
using Portfolio_WPF_App.ViewModels.Handler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace Portfolio_WPF_App.ViewModels
{
    //TODO: Missing Error Bindings
    public class SettingsViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        private ICommand _openConfig;
        private ICommand _saveConfig;
        private ICommand _saveNewUserName;
        private ICommand _saveNewPassword;

        private string _textConfigNameLoaded = "No config openend";
        private string _textConfigLoaded = "";
        private string _textActiveConfigFile = "No config loaded";
        private string _newUsername = "";

        PasswordBox _newPassWordBox;
        PasswordBox _repeatPassWordBox;

        private string _newPassword = "";
        private string _repeatPassword = "";

        public SettingsViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Mediator.Register("OnTextConfigNameLoaded", OnTextConfigNameLoaded);
            Mediator.Register("OnTextConfigLoaded", OnTextConfigLoaded);
            Mediator.Register("OnTextActiveConfigFile", OnTextActiveConfigFile);

            Mediator.Register("OnNewPasswordChanged", OnNewPasswordChanged);
            Mediator.Register("OnNewPassswordEnterPressed", OnNewPassswordEnterPressed);

            Mediator.Register("OnRepeatPasswordChanged", OnRepeatPasswordChanged);
        }

        public ICommand OpenConfig
        {
            get
            {
                if (_openConfig == null)
                {
                    _openConfig = new RelayCommand(
                        param => this.SaveOpenFileCommand(),
                        param => this.CanSaveOpenFileCommand()
                    );
                }
                return _openConfig;
            }
        }

        private bool CanSaveOpenFileCommand()
        {
            return true;
        }

        private void SaveOpenFileCommand()
        {
            OpenFileDialog();
        }

        public ICommand SaveConfig
        {
            get
            {
                if (_saveConfig == null)
                {
                    _saveConfig = new RelayCommand(
                        param => this.SaveConfigCommand(),
                        param => this.CanSaveConfigCommand()
                    );
                }
                return _saveConfig;
            }
        }

        private bool CanSaveConfigCommand()
        {
            return true;
        }

        private void SaveConfigCommand()
        {
            List<object> data = new List<object> { TextConfigNameLoaded, TextConfigLoaded };
            Mediator.NotifyColleagues("ActivateConfigCommand", data);
        }

        public ICommand SaveNewUserName
        {
            get
            {
                if (_saveNewUserName == null)
                {
                    _saveNewUserName = new RelayCommand(
                        param => this.SaveNewUserNameCommand(),
                        param => this.CanSaveNewUserNameCommand()
                    );
                }
                return _saveNewUserName;
            }
        }

        private bool CanSaveNewUserNameCommand()
        {
            return true;
        }

        private void SaveNewUserNameCommand()
        {
            Mediator.NotifyColleagues("SaveNewUserNameCommand", NewUsername);
        }

        public ICommand SaveNewPassword
        {
            get
            {
                if (_saveNewPassword == null)
                {
                    _saveNewPassword = new RelayCommand(
                        param => this.SaveNewPasswordCommand(),
                        param => this.CanSaveNewPasswordCommand()
                    );
                }
                return _saveNewPassword;
            }
        }

        private bool CanSaveNewPasswordCommand()
        {
            return true;
        }

        private void SaveNewPasswordCommand()
        {
            //TODO: Implement pre check of newPassword and repeatPassword
            Mediator.NotifyColleagues("SaveNewPasswordCommand", NewUsername);
        }

        public string TextConfigNameLoaded
        {
            get { return _textConfigNameLoaded; }
            set
            {
                _textConfigNameLoaded = value;
                OnPropertyChanged();
            }
        }

        public void OnTextConfigNameLoaded(object value)
        {
            TextConfigNameLoaded = (string)value;
        }

        public string TextConfigLoaded
        {
            get { return _textConfigLoaded; }
            set
            {
                _textConfigLoaded = value;
                OnPropertyChanged();
            }
        }

        public void OnTextConfigLoaded(object value)
        {
            TextConfigLoaded = (string)value;
        }

        public string TextActiveConfigFile
        {
            get { return _textActiveConfigFile; }
            set
            {
                _textActiveConfigFile = value;
                OnPropertyChanged();
            }
        }

        public void OnTextActiveConfigFile(object value)
        {
            TextActiveConfigFile = (string)value;
        }

        public string NewUsername
        {
            get { return _newUsername; }
            set
            {
                _newUsername = value;
                OnPropertyChanged();
            }
        }

        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
            }
        }

        public void OnNewPasswordChanged(object value)
        {
            _newPassWordBox = (PasswordBox)value;
            NewPassword = (string)_newPassWordBox.Password;
        }

        public void OnNewPassswordEnterPressed(object value)
        {
            SaveNewPasswordCommand();
        }

        public string Password
        {
            get { return _repeatPassword; }
            set
            {
                _repeatPassword = value;
            }
        }

        public void OnRepeatPasswordChanged(object value)
        {
            _repeatPassWordBox = (PasswordBox)value;
            Password = (string)_repeatPassWordBox.Password;
        }

        private void OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Title = "Open Config File";
            openFileDialog.Filter = "config files (*.xml)|*.xml";
            openFileDialog.FilterIndex = 0;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                List<object> data = new List<object> { openFileDialog.FileName, openFileDialog.SafeFileName };
                Mediator.NotifyColleagues("OpenConfigCommand", data);
            }
        }
    }
}
