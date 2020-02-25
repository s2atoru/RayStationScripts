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

            var roiCenterofMasses = new List<Models.RoiCenterOfMass>();

            roiCenterofMasses.Add(new Models.RoiCenterOfMass("PTV1", 1.0, 1.0, 1.0));
            roiCenterofMasses.Add(new Models.RoiCenterOfMass("PTV2", 1.0, 1.0, -1.0));
            roiCenterofMasses.Add(new Models.RoiCenterOfMass("PTV3", 1.0, -1.0, 1.0));
            roiCenterofMasses.Add(new Models.RoiCenterOfMass("PTV4", 1.0, -1.0, -1.0));
            roiCenterofMasses.Add(new Models.RoiCenterOfMass("PTV5", -1.0, 1.0, 1.0));
            roiCenterofMasses.Add(new Models.RoiCenterOfMass("PTV6", -1.0, 1.0, -1.0));
            roiCenterofMasses.Add(new Models.RoiCenterOfMass("PTV7", -1.0, -1.0, 1.0));
            //roiCenterofMasses.Add(new Models.RoiCenterOfMass("PTV8", -1.0, -1.0, -1.0));

            var roiCenterOfMassesViewModel = new ViewModels.RoiCenterOfMassesViewModel(roiCenterofMasses);

            DataContext = roiCenterOfMassesViewModel;
        }
    }
}
