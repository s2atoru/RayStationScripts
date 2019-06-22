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

            List<string> roiNameList = new List<string> { "PTV-RO", "PTV1-RO", "PTV2-RO",
                "PTV", "PTV1", "PTV2",
                "PTV_Low", "PTV_High",
                "CTV", "CTV1", "CTV2",
                "CTV_Low", "CTV_High",
                "zCTV_05", "zCTV_Low_05", "zCTV_High_05",
                "PTVRing", "PTV1Ring", "PTV2Ring",
                "zRingPTV", "zRingPTV_Low", "zRingPTV_High",
                "Bladder outline", "Bladder wall",
                "Bladder", "Bladder_Wall",
                "Rectal outline", "Rectal wall",
                "Rectum", "Rectal_Wall",
                "SeminalVesicle", "SV1/3",
                "SeminalVes", "SeminalVes^1/3",
                "Femoral H_L", "Femoral H_R",
                "Femur_Head_L", "Femur_Head_R",
                "postRectum", "postRectum1", "postRectum2",
                "zPostRectum", "zPostRectum_Low", "zPostRectum_High",
                "optRectum", "optRectum1", "optRectum2",
                "zOptRectum", "zOptRectum_Low", "zOptRectum_High",
                "optBladder", "optBladder1", "optBladder2",
                "zOptBladder", "zOptBladder_Low", "zOptBladder_High",
                "PTVandRO", "PTV1andRO", "PTV2andRO",
                "OL_PTV_Rec", "OL_PTV_Low_Rec", "OL_PTV_High_Rec"};

            roiSelectionViewModel.RoiNameList = roiNameList;
            DataContext = roiSelectionViewModel;
        }

        public MainWindow(ViewModels.RoiSelectionViewModel roiSelectionViewModel)
        {
            InitializeComponent();
            DataContext = roiSelectionViewModel;
        }
    }
}
