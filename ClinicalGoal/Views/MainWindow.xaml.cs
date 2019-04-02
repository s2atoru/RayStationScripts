using System.Windows;
using ClinicalGoal.ViewModels;

namespace ClinicalGoal.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(ClinicalGoalViewModel clinicalGoalViewModel)
        {
            InitializeComponent();

            var mainWindowViewModel = new MainWindowViewModel();
            mainWindowViewModel.ClinicalGoalViewModel = clinicalGoalViewModel;

            DataContext = mainWindowViewModel;
        }
    }
}
