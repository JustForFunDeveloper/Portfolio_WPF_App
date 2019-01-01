using MahApps.Metro.Controls;
using Portfolio_WPF_App.ViewModels.Handler;
using System.Windows.Controls;
using System.Windows.Input;


namespace Portfolio_WPF_App.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : MetroWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            Mediator.NotifyColleagues("OnPasswordChanged", passwordBox);
        }

        private void PasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return && sender != null)
                Mediator.NotifyColleagues("OnEnterPressed", null);
        }
    }
}
