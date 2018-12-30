using Microsoft.Win32;
using Portfolio_WPF_App.ViewModels.Handler;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace Portfolio_WPF_App.ViewModels
{
    //TODO: Implement User & Password Bindings
    public class SettingsViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        private ICommand _openConfig;
        private ICommand _saveConfig;

        private string _textConfigNameLoaded = "No config openend";
        private string _textConfigLoaded = "";
        private string _textActiveConfigFile = "No config loaded";

        public SettingsViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Mediator.Register("OnTextConfigNameLoaded", OnTextConfigNameLoaded);
            Mediator.Register("OnTextConfigLoaded", OnTextConfigLoaded);
            Mediator.Register("OnTextActiveConfigFile", OnTextActiveConfigFile);
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
            List<string> data = new List<string> { TextConfigNameLoaded, TextConfigLoaded };
            Mediator.NotifyColleagues("ActivateConfigCommand", data);
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
                List<string> data = new List<string> { openFileDialog.FileName, openFileDialog.SafeFileName };
                Mediator.NotifyColleagues("OpenConfigCommand", data);
            }
        }
    }
}
