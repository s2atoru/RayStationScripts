using System.Windows;

namespace OptimizatoinSettings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ViewModels.MainWindowViewModel();
        }
    }
}
