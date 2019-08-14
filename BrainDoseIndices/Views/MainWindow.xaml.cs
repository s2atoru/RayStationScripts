using System.Windows;
using System.Collections.Generic;

namespace BrainDoseIndices.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ViewModels.DoseIndicesViewModel();
        }

        public MainWindow(List<Models.StructureDetail> structureDetails)
        {
            InitializeComponent();

            DataContext = new ViewModels.DoseIndicesViewModel(structureDetails);
        }
    }
}
