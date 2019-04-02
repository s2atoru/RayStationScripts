using System.Windows;
using OptimizationRepeater.ViewModels;
using OptimizationRepeater.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        public MainWindow(RepetitionParameters repetitionParameters, List<OptimizationFunction> optimizationFunctions)
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
            ((MainWindowViewModel)DataContext).OptimizationRepeaterViewModel.RepetitionParameters = repetitionParameters;
            ((MainWindowViewModel)DataContext).OptimizationFunctionViewModel.OptimizationFunctions = new ObservableCollection<OptimizationFunction>(optimizationFunctions);
        }
    }
}
