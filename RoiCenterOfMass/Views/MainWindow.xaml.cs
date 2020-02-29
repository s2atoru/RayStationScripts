using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace RoiCenterOfMass.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var roiCenterOfMasses = new List<Models.RoiCenterOfMass>();

            roiCenterOfMasses.Add(new Models.RoiCenterOfMass("PTV1", 1.0, 1.0, 1.0));
            roiCenterOfMasses.Add(new Models.RoiCenterOfMass("PTV2", 1.0, 1.0, -1.0));
            roiCenterOfMasses.Add(new Models.RoiCenterOfMass("PTV3", 1.0, -1.0, 1.0));
            roiCenterOfMasses.Add(new Models.RoiCenterOfMass("PTV4", 1.0, -1.0, -1.0));
            roiCenterOfMasses.Add(new Models.RoiCenterOfMass("PTV5", -1.0, 1.0, 1.0));
            roiCenterOfMasses.Add(new Models.RoiCenterOfMass("PTV6", -1.0, 1.0, -1.0));
            roiCenterOfMasses.Add(new Models.RoiCenterOfMass("PTV7", -1.0, -1.0, 1.0));
            //roiCenterofMasses.Add(new Models.RoiCenterOfMass("PTV8", -1.0, -1.0, -1.0));

            var roiCenterOfMassesViewModel = new ViewModels.RoiCenterOfMassesViewModel(roiCenterOfMasses);

            DataContext = roiCenterOfMassesViewModel;
        }

        public MainWindow(List<Models.RoiCenterOfMass> roiCenterOfMasses)
        {
            InitializeComponent();

            var roiCenterOfMassesViewModel = new ViewModels.RoiCenterOfMassesViewModel(roiCenterOfMasses);

            DataContext = roiCenterOfMassesViewModel;
        }
    }
}
