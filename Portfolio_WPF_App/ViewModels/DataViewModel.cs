using Microsoft.Win32;
using Portfolio_WPF_App.Core.DataModel;
using Portfolio_WPF_App.Core.Handler;
using Portfolio_WPF_App.DataModel;
using Portfolio_WPF_App.ViewModels.Handler;
using Portfolio_WPF_App.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
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

        private Visibility _hideScrollViewer = Visibility.Visible;
        private Visibility _hideDataGrid = Visibility.Hidden;

        private ICommand _saveLog;

        private string _textLogFileName = "";
        private string _textLogFile = "";

        private bool _DEBUGChecked = false;
        private bool _INFOChecked = false;
        private bool _WARNINGChecked = false;
        private bool _ERRORChecked = false;

        private int _splitButtonIndex = 0;

        private CultureInfo _cultureFormat;
        private DateTime _selectedDateTime;

        public ObservableCollection<DataButton> SplitButtonItems { get; } = new ObservableCollection<DataButton>();
        public ObservableCollection<Data> DataCollection { get; } = new ObservableCollection<Data>();

        public DataViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Mediator.Register("OnAdminLogin", OnAdminLogin);
            Mediator.Register("OnLogLevel", OnLogLevel);
            Mediator.Register("OnDataChange", OnDataChange);

            Mediator.Register("OnLogData", OnLogData);
            Mediator.Register("OnData", OnData);

            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
            culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            CultureFormat = culture;

            SelectedDateTime = DateTime.Today;

            SplitButtonItems.Add(new DataButton("Logs"));
            SplitButtonItems.Add(new DataButton("Data"));
        }

        private string GetSelectedItemAsString(Data data, string Header)
        {
            switch (Header)
            {
                case "Id":
                    return data.Id.ToString();
                case "Time":
                    return data.Time;
                case "Name":
                    return data.Name;
                case "Surename":
                    return data.SureName;
                case "Message":
                    return data.Message;
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

        public Visibility HideScrollViewer
        {
            get { return _hideScrollViewer; }
            set
            {
                _hideScrollViewer = value;
                OnPropertyChanged();
            }
        }

        public Visibility HideDataGrid
        {
            get { return _hideDataGrid; }
            set
            {
                _hideDataGrid = value;
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

        public string TextLogFile
        {
            get { return _textLogFile; }
            set
            {
                _textLogFile = value;
                OnPropertyChanged();
            }
        }

        public void OnLogData(object value)
        {
            ListArguments listArguments = (ListArguments)value;
            TextLogFileName = (string)listArguments.Value[0];
            TextLogFile = String.Join(Environment.NewLine, listArguments.Value);
        }

        private void OnData(object value)
        {
            DataCollection.Clear();
            ListArguments listArguments = (ListArguments)value;
            List<object> listData = listArguments.Value;
            foreach (object item in listData)
            {
                DataCollection.Add((Data)item);
            }
        }

        private void OpenSaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            string timeString = DateTime.Now.ToString();
            timeString = timeString.Replace(':', '-');
            timeString = timeString.Replace(' ', '_');
            saveFileDialog.FileName = timeString;
            if (SplitButtonIndex.Equals(0))
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    saveFileDialog.Title = "Save Log File";
                    List<object> data = new List<object> { saveFileDialog.SafeFileName, TextLogFile };
                    Mediator.NotifyColleagues("SaveDataCommand", data);
                }
            }
            else
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    saveFileDialog.Title = "Save Data File";
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (Data item in DataCollection)
                    {
                        stringBuilder.AppendLine(item.ToString());
                    }

                    List<object> data = new List<object> { saveFileDialog.SafeFileName, stringBuilder.ToString() };
                    Mediator.NotifyColleagues("SaveDataCommand", data);
                }
            }
        }

        public bool DEBUGChecked
        {
            get { return _DEBUGChecked; }
            set
            {
                _DEBUGChecked = value;
                SplitButtonIndex = 0;
                LoadLogDataViewBasedOnFilterStatus();
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
                SplitButtonIndex = 0;
                LoadLogDataViewBasedOnFilterStatus();
                OnPropertyChanged();
            }
        }

        public bool WARNINGChecked
        {
            get { return _WARNINGChecked; }
            set
            {
                _WARNINGChecked = value;
                SplitButtonIndex = 0;
                LoadLogDataViewBasedOnFilterStatus();
                OnPropertyChanged();
            }
        }

        public bool ERRORChecked
        {
            get { return _ERRORChecked; }
            set
            {
                _ERRORChecked = value;
                SplitButtonIndex = 0;
                LoadLogDataViewBasedOnFilterStatus();
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
            SplitButtonIndex = 0;
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

        public int SplitButtonIndex
        {
            get { return _splitButtonIndex; }
            set
            {
                _splitButtonIndex = value;
                if (value.Equals(0))
                    HideDataGridAndShowScrollViewer(true);
                else
                    HideDataGridAndShowScrollViewer(false);
                    OnPropertyChanged();
            }
        }

        private void OnDataChange(object value)
        {
            SplitButtonIndex = (int)value;
        }

        private void HideDataGridAndShowScrollViewer(bool value)
        {
            if (value)
            {
                HideDataGrid = Visibility.Hidden;
                HideScrollViewer = Visibility.Visible;
                LoadLogDataViewBasedOnFilterStatus();
            }
            else
            {
                HideDataGrid = Visibility.Visible;
                HideScrollViewer = Visibility.Hidden;
                TextLogFileName = "DATA BASE";
            }
        }

        private void LoadLogDataViewBasedOnFilterStatus()
        {
            TextLogFileName = "??";
            if (AllCheckBoxesChecked())
                Mediator.NotifyColleagues("RequestLogDataCommand", null);
            else
            {
                List<object> logLevelsChecked = new List<object>();
                if (DEBUGChecked)
                    logLevelsChecked.Add(LogLevel.DEBUG);
                if (INFOChecked)
                    logLevelsChecked.Add(LogLevel.INFO);
                if (WARNINGChecked)
                    logLevelsChecked.Add(LogLevel.WARNING);
                if (ERRORChecked)
                    logLevelsChecked.Add(LogLevel.ERROR);

                Mediator.NotifyColleagues("RequestLogLevelFilteredDataCommand", logLevelsChecked);
            }
        }

        private bool AllCheckBoxesChecked()
        {
            if (DEBUGChecked && INFOChecked && WARNINGChecked && ERRORChecked)
                return true;
            else if (!DEBUGChecked && !INFOChecked && !WARNINGChecked && !ERRORChecked)
                return true;
            else
                return false;
        }

    }
}
