using Portfolio_WPF_App.GlobalValues;
using Portfolio_WPF_App.ViewModels.Handler;
using System;
using System.Windows.Input;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace Portfolio_WPF_App.ViewModels
{
    //TODO: Bind EMailText and WebsiteText Property
    public class AboutViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        private string _textVersion = "V ?";
        private string _textLogicTitle = "Title ?";
        private string _textLogicVersion = "V ?";

        private ICommand _openEmail;
        private ICommand _openWeb;

        public AboutViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Mediator.Register("OnTextVersion", OnTextVersion);
            Mediator.Register("OnTextLogicTitle", OnTextLogicTitle);
            Mediator.Register("OnTextLogicVersion", OnTextLogicVersion);
        }

        public string TextVersion
        {
            get { return _textVersion; }
            set
            {
                _textVersion = value;
                OnPropertyChanged();
            }
        }

        public void OnTextVersion(object value)
        {
            TextVersion = (string)value;
        }

        public string TextLogicTitle
        {
            get { return _textLogicTitle; }
            set
            {
                _textLogicTitle = value;
                OnPropertyChanged();
            }
        }

        public void OnTextLogicTitle(object value)
        {
            TextLogicTitle = (string)value;
        }

        public string TextLogicVersion
        {
            get { return _textLogicVersion; }
            set
            {
                _textLogicVersion = value;
                OnPropertyChanged();
            }
        }


        public ICommand OpenEmail
        {
            get
            {
                if (_openEmail == null)
                {
                    _openEmail = new RelayCommand(
                            param => this.SaveOpenEmailCommand(),
                            param => this.CanOpenEmailCommand()
                        );
                }
                return _openEmail;
            }
        }

        private bool CanOpenEmailCommand()
        {
            return true;
        }

        private void SaveOpenEmailCommand()
        {
            try
            {
                Outlook.Application oApp = new Outlook.Application();
                Outlook._MailItem oMailItem = (Outlook._MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);
                oMailItem.To = GlobalConstants.COMPANY_MAIL_ADDRESS;
                oMailItem.Display(true);
            }
            catch (Exception e)
            {
                // TODO: Log Exception
            }
        }

        public ICommand OpenWeb
        {
            get
            {
                if (_openWeb == null)
                {
                    _openWeb = new RelayCommand(
                            param => this.SaveOpenWebCommand(),
                            param => this.CanOpenWebCommand()
                        );
                }
                return _openWeb;
            }
        }

        private bool CanOpenWebCommand()
        {
            return true;
        }

        private void SaveOpenWebCommand()
        {
            System.Diagnostics.Process.Start(GlobalConstants.COMPANY_WEB_ADDRESS);
        }

        public void OnTextLogicVersion(object value)
        {
            TextLogicVersion = (string)value;
        }
    }
}
