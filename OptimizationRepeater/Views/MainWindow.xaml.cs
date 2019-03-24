using System.Windows;
using OptimizationRepeater.ViewModels;
using OptimizationRepeater.Models;

namespace OptimizationRepeater.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

        public MainWindow(RepetitionParameters repetitionParameters)
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
            ((MainWindowViewModel)DataContext).OptimizationRepeaterViewModel.RepetitionParameters = repetitionParameters;
        }
    }
}
