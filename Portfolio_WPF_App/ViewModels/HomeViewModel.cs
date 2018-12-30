using Portfolio_WPF_App.Core.Handler;
using Portfolio_WPF_App.ViewModels.Handler;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Portfolio_WPF_App.ViewModels
{
    public class HomeViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        private bool _dbConnected = false;

        private string _txtMsginDB = "0";
        private string _textError = "0";
        private string _textWarning = "0";
        private string _textInfo = "0";

        private string _textDBTooltip = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");

        private ICommand _onErrorMessages;
        private ICommand _onWarningMessages;
        private ICommand _onInfoMessages;
        private ICommand _onMsginDB;

        public HomeViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Mediator.Register("OnDbConnected", OnDbConnected);

            Mediator.Register("OnTextMsgInDB", OnTextMsgInDB);

            Mediator.Register("OnTextError", OnTextError);
            Mediator.Register("OnTextWarning", OnTextWarning);
            Mediator.Register("OnTextInfo", OnTextInfo);
        }

        public bool DbConnected
        {
            get { return _dbConnected; }
            set
            {
                _dbConnected = value;
                OnPropertyChanged();
            }
        }

        public void OnDbConnected(object value)
        {
            DbConnected = (bool)value;
            TextDBTooltip = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
        }

        public string TextMsgInDB
        {
            get { return _txtMsginDB; }
            set
            {
                _txtMsginDB = value;
                OnPropertyChanged();
            }
        }

        public void OnTextMsgInDB(object value)
        {
            TextMsgInDB = (string)value;
        }

        public string TextError
        {
            get { return _textError; }
            set
            {
                _textError = value;
                OnPropertyChanged();
            }
        }

        public void OnTextError(object value)
        {
            TextError = (string)value;
        }

        public string TextWarning
        {
            get { return _textWarning; }
            set
            {
                _textWarning = value;
                OnPropertyChanged();
            }
        }

        public void OnTextWarning(object value)
        {
            TextWarning = (string)value;
        }

        public string TextInfo
        {
            get { return _textInfo; }
            set
            {
                _textInfo = value;
                OnPropertyChanged();
            }
        }

        public void OnTextInfo(object value)
        {
            TextInfo = (string)value;
        }

        public string TextDBTooltip
        {
            get { return _textDBTooltip; }
            set
            {
                _textDBTooltip = value;
                OnPropertyChanged();
            }
        }

        public ICommand OnMsginDB
        {
            get
            {
                if (_onMsginDB == null)
                {
                    _onMsginDB = new RelayCommand(
                            param => this.SaveOnMsginDB(),
                            param => this.CanOnMsginDB()
                        );
                }
                return _onMsginDB;
            }
        }

        private bool CanOnMsginDB()
        {
            return true;
        }

        private void SaveOnMsginDB()
        {
            KeyValuePair<int, int> value = new KeyValuePair<int, int>(2, 1);
            Mediator.NotifyColleagues("OnViewDataChange", value);
        }

        public ICommand OnErrorMessages
        {
            get
            {
                if (_onErrorMessages == null)
                {
                    _onErrorMessages = new RelayCommand(
                            param => this.SaveOnErrorMessagesCommand(),
                            param => this.CanOnErrorMessagesCommand()
                        );
                }
                return _onErrorMessages;
            }
        }

        private bool CanOnErrorMessagesCommand()
        {
            return true;
        }

        private void SaveOnErrorMessagesCommand()
        {
            KeyValuePair<int, LogLevel> value = new KeyValuePair<int, LogLevel>(2, LogLevel.ERROR);
            Mediator.NotifyColleagues("OnViewLogLevelChange", value);
        }

        public ICommand OnWarningMessages
        {
            get
            {
                if (_onWarningMessages == null)
                {
                    _onWarningMessages = new RelayCommand(
                            param => this.SaveOnWarningMessagesCommand(),
                            param => this.CanOnWarningMessagesCommand()
                        );
                }
                return _onWarningMessages;
            }
        }

        private bool CanOnWarningMessagesCommand()
        {
            return true;
        }

        private void SaveOnWarningMessagesCommand()
        {
            KeyValuePair<int, LogLevel> value = new KeyValuePair<int, LogLevel>(2, LogLevel.WARNING);
            Mediator.NotifyColleagues("OnViewLogLevelChange", value);
        }

        public ICommand OnInfoMessages
        {
            get
            {
                if (_onInfoMessages == null)
                {
                    _onInfoMessages = new RelayCommand(
                            param => this.SaveOnInfoMessagesCommand(),
                            param => this.CanOnInfoMessagesCommand()
                        );
                }
                return _onInfoMessages;
            }
        }

        private bool CanOnInfoMessagesCommand()
        {
            return true;
        }

        private void SaveOnInfoMessagesCommand()
        {
            KeyValuePair<int, LogLevel> value = new KeyValuePair<int, LogLevel>(2, LogLevel.INFO);
            Mediator.NotifyColleagues("OnViewLogLevelChange", value);
        }

        private void OntextDBTooltip(object value)
        {
            DateTime dateTime = (DateTime)value;
            _textDBTooltip = dateTime.ToString("yyyy-MM-dd hh:mm:ss.fff"); ;
        }
    }
}
