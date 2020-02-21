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

            roiCenterofMasses.Add(new Models.RoiCenterOfMass { Id = "PTV1", Coordinates = new Point3D(1.0, 1.0, 1.0) });
            roiCenterofMasses.Add(new Models.RoiCenterOfMass { Id = "PTV2", Coordinates = new Point3D(1.0, 1.0, -1.0) });
            roiCenterofMasses.Add(new Models.RoiCenterOfMass { Id = "PTV3", Coordinates = new Point3D(1.0, -1.0, 1.0) });
            roiCenterofMasses.Add(new Models.RoiCenterOfMass { Id = "PTV4", Coordinates = new Point3D(1.0, -1.0, -1.0) });
            roiCenterofMasses.Add(new Models.RoiCenterOfMass { Id = "PTV5", Coordinates = new Point3D(-1.0, 1.0, 1.0) });
            roiCenterofMasses.Add(new Models.RoiCenterOfMass { Id = "PTV6", Coordinates = new Point3D(-1.0, 1.0, -1.0) });
            roiCenterofMasses.Add(new Models.RoiCenterOfMass { Id = "PTV7", Coordinates = new Point3D(-1.0, -1.0, 1.0) });
            roiCenterofMasses.Add(new Models.RoiCenterOfMass { Id = "PTV8", Coordinates = new Point3D(-1.0, -1.0, -1.0) });

            var roiCenterOfMassesViewModel = new ViewModels.RoiCenterOfMassesViewModel(roiCenterofMasses);

            DataContext = roiCenterOfMassesViewModel;
        }
    }
}
