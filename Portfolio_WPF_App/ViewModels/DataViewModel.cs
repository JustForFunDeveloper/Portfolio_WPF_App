using Microsoft.Win32;
using Portfolio_WPF_App.Core.Handler;
using Portfolio_WPF_App.DataModel;
using Portfolio_WPF_App.ViewModels.Handler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Portfolio_WPF_App.ViewModels
{
    class DataViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        private Visibility _hideAdminNotice = Visibility.Visible;
        private Visibility _hideAdminContent = Visibility.Hidden;

        private ICommand _saveLog;
        private ICommand _loadData;

        private string _textLogFileName = "No config loaded";
        private string _textLogFile = "No config loaded";

        private bool _DEBUGChecked = false;
        private bool _INFOChecked = false;
        private bool _WARNINGChecked = false;
        private bool _ERRORChecked = false;

        private CultureInfo _cultureFormat;
        private DateTime _selectedDateTime;

        private DispatcherTimer timer;
        private int counter = 1;

        public ObservableCollection<Log> LogCollection { get; } = new ObservableCollection<Log>();

        public DataViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Mediator.Register("OnAdminLogin", OnAdminLogin);
            Mediator.Register("OnTextLogFileName", OnTextLogFileName);
            Mediator.Register("OnTextLogFile", OnTextLogFile);
            Mediator.Register("OnLogLevel", OnLogLevel);

            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
            culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            CultureFormat = culture;

            SelectedDateTime = DateTime.Today;

            timer = new DispatcherTimer(DispatcherPriority.Background, Application.Current.Dispatcher);
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;

            Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (LogCollection.Count >= 29)
            {
                Stop();
            }
            var newLog = new Log
            {
                Id = counter,
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                MessageLevel = LogLevel.DEBUG.ToString(),
                Message = "Das ist eine laaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaa aaaaaaaaaaaaaa aaaaaaaaaaa aaaaaaaange eine Test Message"
            };
            Action action = () => { LogCollection.Add(newLog); };
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(action));
            counter++;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        private string GetSelectedItemAsString(Log log, string Header)
        {
            switch (Header)
            {
                case "Id":
                    return log.Id.ToString();
                case "Time":
                    return log.Time;
                case "Message Level":
                    return log.MessageLevel;
                case "Message":
                    return log.Message;
                default:
                    return "";
            }
        }

        public Visibility HideAdminNotice
        {
            get { return _hideAdminNotice; }
            set
            {
                _hideAdminNotice = value;
                OnPropertyChanged();
            }
        }

        public Visibility HideAdminContent
        {
            get { return _hideAdminContent; }
            set
            {
                _hideAdminContent = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveLog
        {
            get
            {
                if (_saveLog == null)
                {
                    _saveLog = new RelayCommand(
                        param => this.SaveLogCommand(),
                        param => this.CanSaveLogCommand()
                    );
                }
                return _saveLog;
            }
        }

        private bool CanSaveLogCommand()
        {
            return true;
        }

        private void SaveLogCommand()
        {
            OpenSaveFileDialog();
        }

        public void OnAdminLogin(object show)
        {
            bool loggedIn = (bool)show;
            if (loggedIn)
            {
                HideAdminNotice = Visibility.Hidden;
                HideAdminContent = Visibility.Visible;
            }
            else
            {
                HideAdminNotice = Visibility.Visible;
                HideAdminContent = Visibility.Hidden;
            }
        }

        public string TextLogFileName
        {
            get { return _textLogFileName; }
            set
            {
                _textLogFileName = value;
                OnPropertyChanged();
            }
        }

        public void OnTextLogFileName(object value)
        {
            TextLogFileName = (string)value;
        }

        public string TextLogFile
        {
            get { return _textLogFile; }
            set
            {
                _textLogFile = value;
                OnPropertyChanged();
            }
        }

        public void OnTextLogFile(object value)
        {
            TextLogFile = (string)value;
        }

        private void OpenSaveFileDialog()
        {
            if (TextLogFile.Length.Equals(0))
            {
                Console.WriteLine("Log is empty!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog.Title = "Save Log File";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            string timeString = DateTime.Now.ToString();
            timeString = timeString.Replace(':', '-');
            timeString = timeString.Replace(' ', '_');
            saveFileDialog.FileName = timeString;
            if (saveFileDialog.ShowDialog() == true)
            {
                List<string> data = new List<string> { saveFileDialog.SafeFileName, TextLogFile };
                Mediator.NotifyColleagues("SaveLogCommand", data);
            }
        }

        public bool DEBUGChecked
        {
            get { return _DEBUGChecked; }
            set
            {
                _DEBUGChecked = value;
                OnPropertyChanged();
            }
        }

        private void OnDEBUGChecked(object value)
        {
            DEBUGChecked = (bool)value;
        }

        public bool INFOChecked
        {
            get { return _INFOChecked; }
            set
            {
                _INFOChecked = value;
                OnPropertyChanged();
            }
        }

        public bool WARNINGChecked
        {
            get { return _WARNINGChecked; }
            set
            {
                _WARNINGChecked = value;
                OnPropertyChanged();
            }
        }

        public bool ERRORChecked
        {
            get { return _ERRORChecked; }
            set
            {
                _ERRORChecked = value;
                OnPropertyChanged();
            }
        }

        private void OnLogLevel(object value)
        {
            LogLevel logLevel = (LogLevel)value;

            switch (logLevel)
            {
                case LogLevel.DEBUG:
                    DEBUGChecked = true;
                    INFOChecked = false;
                    WARNINGChecked = false;
                    ERRORChecked = false;
                    break;
                case LogLevel.INFO:
                    DEBUGChecked = false;
                    INFOChecked = true;
                    WARNINGChecked = false;
                    ERRORChecked = false;
                    break;
                case LogLevel.WARNING:
                    DEBUGChecked = false;
                    INFOChecked = false;
                    WARNINGChecked = true;
                    ERRORChecked = false;
                    break;
                case LogLevel.ERROR:
                    DEBUGChecked = false;
                    INFOChecked = false;
                    WARNINGChecked = false;
                    ERRORChecked = true;
                    break;
                default:
                    break;
            }
        }

        public CultureInfo CultureFormat
        {
            get { return _cultureFormat; }
            set
            {
                _cultureFormat = value;
                OnPropertyChanged();
            }
        }

        public DateTime SelectedDateTime
        {
            get { return _selectedDateTime; }
            set
            {
                _selectedDateTime = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadData
        {
            get
            {
                if (_loadData == null)
                {
                    _loadData = new RelayCommand(
                        param => this.LoadDataCommand(),
                        param => this.CanLoadDataCommand()
                    );
                }
                return _loadData;
            }
        }

        private bool CanLoadDataCommand()
        {
            return true;
        }

        private void LoadDataCommand()
        {
            Console.WriteLine("LoadDataCommand: " + _selectedDateTime.ToString("yyy-MM-dd HH:mm:ss.fff"));
        }

    }
}
