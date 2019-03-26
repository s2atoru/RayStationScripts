using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;

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
    }
}
