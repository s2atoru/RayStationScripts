using System.Linq;
using System.Windows;

namespace MlcShifter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var mlcShifterViewModel = new ViewModels.MlcShifterViewModel();

            DataContext = mlcShifterViewModel;

            mlcShifterViewModel.SelectedBeamId = (mlcShifterViewModel.BeamViewModels.First()).BeamId;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
