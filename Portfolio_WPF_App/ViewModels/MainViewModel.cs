using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Portfolio_WPF_App.Core.Handler;
using Portfolio_WPF_App.ViewModels.Handler;
using Portfolio_WPF_App.Views;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Portfolio_WPF_App.ViewModels
{
    public class MainViewModel : PropertyChangedViewModel
    {
        private static MainViewModel _mainViewModel;

        private Visibility _hideWindowCommands = Visibility.Visible;

        private ICommand _logIn;
        private ICommand _closeWindow;
        private ICommand _itemClick;
        private ICommand _optionItemClick;

        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuItemCollection _menuOptionItems;

        private bool _isPaneOpened;
        private bool _isLoggedIn;
        private int _selectedIndex = 0;

        private MetroWindow _loginWindow;

        public MainViewModel()
        {
            this.CreateMenuItems();
            _mainViewModel = this;
            Mediator.Register("OnLoginTry", LogInUser);
            Mediator.Register("CloseLoginWindow", HideLoginWindow);
            Mediator.Register("OnViewLogLevelChange", OnViewLogLevelChange);
            Mediator.Register("OnViewDataChange", OnViewDataChange);
        }

        public static MainViewModel getInstance { get => _mainViewModel; }

        public void CreateMenuItems()
        {
            MenuItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Home},
                    Label = "Home",
                    ToolTip = "The Home view.",
                    Tag = new HomeViewModel(this)
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Settings},
                    Label = "Settings",
                    ToolTip = "The Application settings.",
                    Tag = new SettingsViewModel(this)
                },
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.ClipboardPlus },
                    Label = "Data",
                    ToolTip = "Shows the database data.",
                    Tag = new DataViewModel(this)
                }
            };

            MenuOptionItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Help},
                    Label = "About",
                    ToolTip = "About this Software.",
                    Tag = new AboutViewModel(this)
                }
            };
        }

        public Visibility HideWindowCommands
        {
            get { return _hideWindowCommands; }
            set
            {
                _hideWindowCommands = value;
                OnPropertyChanged();
            }
        }

        public HamburgerMenuItemCollection MenuItems
        {
            get { return _menuItems; }
            set
            {
                if (Equals(value, _menuItems)) return;
                _menuItems = value;
                OnPropertyChanged();
            }
        }

        public HamburgerMenuItemCollection MenuOptionItems
        {
            get { return _menuOptionItems; }
            set
            {
                if (Equals(value, _menuOptionItems)) return;
                _menuOptionItems = value;
                OnPropertyChanged();
            }
        }

        public bool IsPaneOpened
        {
            get { return _isPaneOpened; }
            set
            {
                _isPaneOpened = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
            }
        }

        public ICommand LogIn
        {
            get
            {
                if (_logIn == null)
                {
                    _logIn = new RelayCommand(
                        param => this.SaveLogInCommand(),
                        param => this.CanSaveLogInCommand()
                    );
                }
                return _logIn;
            }
        }

        private bool CanSaveLogInCommand()
        {
            return true;
        }

        private void SaveLogInCommand()
        {
            if (IsLoggedIn)
            {
                LogInUser(false);
                return;
            }

            if (_loginWindow != null)
            {
                _loginWindow.Show();

            }
            else
            {
                _loginWindow = new LoginWindow();
                _loginWindow.Closed += (o, args) => _loginWindow = null;
                _loginWindow.Left = MainWindow.GetInstance.Left + MainWindow.GetInstance.ActualWidth / 2.0;
                _loginWindow.Top = MainWindow.GetInstance.Top + MainWindow.GetInstance.ActualHeight / 2.0;
                _loginWindow.ShowDialog();
            }
        }

        public ICommand CloseWindow
        {
            get
            {
                if (_closeWindow == null)
                {
                    _closeWindow = new RelayCommand(
                        param => this.SaveCloseWindowCommand(),
                        param => this.CanSaveCloseWindowCommand()
                    );
                }
                return _closeWindow;
            }
        }

        private bool CanSaveCloseWindowCommand()
        {
            return true;
        }

        private void SaveCloseWindowCommand()
        {
            if (_loginWindow != null)
                _loginWindow.Close();
        }

        public ICommand ItemClick
        {
            get
            {
                if (_itemClick == null)
                {
                    _itemClick = new RelayCommand<int>(
                        param => this.ItemClickCommand(param),
                        param => this.CanItemClickCommand(param)
                    );
                }
                return _itemClick;
            }
        }

        private bool CanItemClickCommand(int param)
        {
            return true;
        }

        private void ItemClickCommand(int param)
        {
            IsPaneOpened = false;
            switch (param)
            {
                case 0:
                    Mediator.NotifyColleagues("ReloadHomeView", true);
                    break;
                case 1:
                    Mediator.NotifyColleagues("ReloadSettingsView", true);
                    break;
                case 2:
                    Mediator.NotifyColleagues("ReloadDataView", true);
                    break;
                default:
                    break;
            }
        }

        public ICommand OptionItemClick
        {
            get
            {
                if (_optionItemClick == null)
                {
                    _optionItemClick = new RelayCommand<int>(
                        param => this.OptionItemClickCommand(param),
                        param => this.CanOptionItemClickCommand(param)
                    );
                }
                return _optionItemClick;
            }
        }

        private bool CanOptionItemClickCommand(int param)
        {
            return true;
        }

        private void OptionItemClickCommand(int param)
        {
            IsPaneOpened = false;
            //Reload AboutView not needed at the moment

            //HamburgerMenu hamburgerMenu = (HamburgerMenu)sender;
            //switch (hamburgerMenu.SelectedOptionsIndex)
            //{
            //    case 0:
            //        Mediator.NotifyColleagues("ReloadAboutView", true);
            //        break;
            //    default:
            //        break;
            //}
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        private void LogInUser(object value)
        {
            IsLoggedIn = (bool)value;
            Mediator.NotifyColleagues("OnAdminLogin", IsLoggedIn);
            HideLoginWindow(null);
        }

        private void HideLoginWindow(object value)
        {
            if (_loginWindow != null)
                _loginWindow.Hide();
        }

        private void OnViewLogLevelChange(object value)
        {
            KeyValuePair<int, LogLevel> keyValuePair = (KeyValuePair<int, LogLevel>)value;
            SelectedIndex = keyValuePair.Key;
            Mediator.NotifyColleagues("OnLogLevel", keyValuePair.Value);
        }

        private void OnViewDataChange(object value)
        {
            KeyValuePair<int, int> keyValuePair = (KeyValuePair<int, int>)value;
            SelectedIndex = keyValuePair.Key;
            Mediator.NotifyColleagues("OnDataChange", keyValuePair.Value);
        }
    }
}
