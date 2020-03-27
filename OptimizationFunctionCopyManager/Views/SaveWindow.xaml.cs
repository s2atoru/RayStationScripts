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
    /// SaveWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SaveWindow : Window
    {
        public SaveWindow()
        {
            InitializeComponent();

            var saveObjectiveFunctionsViewModel = new ViewModels.SaveObjectiveFunctionsViewModel();

            DataContext = saveObjectiveFunctionsViewModel;
        }

        public SaveWindow(string objectiveFunctionsJson, List<Models.Prescription> prescriptions, string defaultDirectoryPath)
        {
            InitializeComponent();

            var saveObjectiveFunctionsViewModel = new ViewModels.SaveObjectiveFunctionsViewModel(objectiveFunctionsJson, defaultDirectoryPath);

            DataContext = saveObjectiveFunctionsViewModel;
        }
    }
}
