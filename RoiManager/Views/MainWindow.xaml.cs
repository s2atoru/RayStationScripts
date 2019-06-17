using System.Collections.Generic;
using System.Windows;

namespace RoiManager.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var rois = new List<Models.Roi>();

            rois.Add(new Models.Roi { Name = "PTV-CTV", IsDerived = true, DependentRois = new List<string> { "PTV", "CTV" }, HasGeometry = true, CanDeleteGeometry = false, CanUnderive = false, CaseName = "C1", ExaminationName = "CT1" });
            rois.Add(new Models.Roi { Name = "PTV", IsDerived = false, DependentRois = new List<string> { }, HasGeometry = true, CanDeleteGeometry = false, CanUnderive = false, CaseName = "C1", ExaminationName = "CT1" });

            var roiSelectionViewModel = new ViewModels.RoiSelectionViewModel(rois);

            DataContext = roiSelectionViewModel;
        }

        public MainWindow(ViewModels.RoiSelectionViewModel roiSelectionViewModel)
        {
            InitializeComponent();
            DataContext = roiSelectionViewModel;
        }
    }
}
