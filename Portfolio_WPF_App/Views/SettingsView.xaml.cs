using Portfolio_WPF_App.ViewModels.Handler;
using System.Windows.Controls;
using System.Windows.Input;

namespace Portfolio_WPF_App.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void NewPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            Mediator.NotifyColleagues("OnNewPasswordChanged", passwordBox);
        }

        private void NewPasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return && sender != null)
                Mediator.NotifyColleagues("OnNewPassswordEnterPressed", null);
        }

        private void RepeatPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            Mediator.NotifyColleagues("OnRepeatPasswordChanged", passwordBox);
        }

        private void RepeatPasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return && sender != null)
                Mediator.NotifyColleagues("OnNewPassswordEnterPressed", null);
        }
    }
}
