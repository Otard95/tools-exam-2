using System.Windows;
using LevelEditor.ViewModel;

namespace LevelEditor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow () {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Maximized;
        }
    }
}