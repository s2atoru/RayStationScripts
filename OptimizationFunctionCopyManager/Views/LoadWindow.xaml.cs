using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OptimizationFunctionCopyManager.Views
{
    /// <summary>
    /// Load.xaml の相互作用ロジック
    /// </summary>
    public partial class LoadWindow : Window
    {
        public LoadWindow()
        {
            InitializeComponent();

            var loadObjectiveFunctionsViewModel = new ViewModels.LoadObjectiveFunctionsViewModel();

            loadObjectiveFunctionsViewModel.DefaultDirectoryPath = @"C:\Users\s2ato\Desktop\RayStationScripts\OptimizationFunctions";

            DataContext = loadObjectiveFunctionsViewModel;
        }

        public LoadWindow(List<Models.Roi> rois, List<Models.PlanLabel> planLabels, string defaultDirectoryPath)
        {
            InitializeComponent();

            var loadObjectiveFunctionsViewModel = new ViewModels.LoadObjectiveFunctionsViewModel(rois, planLabels, defaultDirectoryPath);

            DataContext = loadObjectiveFunctionsViewModel;
        }
    }
}
