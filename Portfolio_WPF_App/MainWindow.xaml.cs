using MahApps.Metro.Controls;
using Portfolio_WPF_App.Control;

namespace Portfolio_WPF_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static MetroWindow _mainView;

        public MainWindow()
        {
            InitializeComponent();
            _mainView = this;
            new Adapter();
        }

        public static MetroWindow GetInstance { get => _mainView; }
    }
}
