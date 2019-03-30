using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace RoiFormulaMaker.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new ViewModels.MainWindowViewModel();
        }

        public MainWindow(List<string> structureNames, List<dynamic> structureDesigns)
        {
            InitializeComponent();

            var viewModel = new ViewModels.MainWindowViewModel
            {
                StructureNames = new ObservableCollection<string>(structureNames),
                StructureDesigns = structureDesigns
            };

            this.DataContext = viewModel;
        }

        public MainWindow(List<string> structureNames, List<dynamic> structureDesigns, string defaultDirectoryPath)
        {
            InitializeComponent();

            var viewModel = new ViewModels.MainWindowViewModel
            {
                StructureNames = new ObservableCollection<string>(structureNames),
                StructureDesigns = structureDesigns,
                DefaultDirectoryPath = defaultDirectoryPath
            };

            this.DataContext = viewModel;
        }

        public MainWindow(Dictionary<string, Dictionary<string, object>> structureDetails, List<dynamic> structureDesigns, string defaultDirectoryPath)
        {
            InitializeComponent();

            var contouredStructureNames = new List<string>();

            foreach (var item in structureDetails)
            {
                if ((bool)item.Value["HasContours"])
                {
                    contouredStructureNames.Add(item.Key);
                }
            }

            var viewModel = new ViewModels.MainWindowViewModel
            {
                StructureDetails = structureDetails,
                StructureDesigns = structureDesigns,
                DefaultDirectoryPath = defaultDirectoryPath
            };

            this.DataContext = viewModel;
        }
    }
}
