using RoiFormulaMaker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

            var mainWindowViewModel = new ViewModels.MainWindowViewModel();
            mainWindowViewModel.structureFormulas = mainWindowViewModel.RoiFormulas.Formulas;

            mainWindowViewModel.DefaultDirectoryPath = Path.Combine(Environment.GetEnvironmentVariable("HOMEPATH"), @"Desktop\RayStationScripts\RoiFormulas");
            this.DataContext = mainWindowViewModel;
        }

        public MainWindow(List<string> structureNames, List<dynamic> structureFormulas)
        {
            InitializeComponent();

            var viewModel = new ViewModels.MainWindowViewModel
            {
                StructureNames = new ObservableCollection<string>(structureNames),
                StructureFormulas = structureFormulas
            };

            this.DataContext = viewModel;
        }

        public MainWindow(List<string> structureNames, List<dynamic> structureFormulas, string defaultDirectoryPath)
        {
            InitializeComponent();

            var viewModel = new ViewModels.MainWindowViewModel
            {
                StructureNames = new ObservableCollection<string>(structureNames),
                StructureFormulas = structureFormulas,
                DefaultDirectoryPath = defaultDirectoryPath
            };

            this.DataContext = viewModel;
        }

        public MainWindow(Dictionary<string, Dictionary<string, object>> structureDetails, RoiFormulas roiFormulas, string defaultDirectoryPath)
        {
            InitializeComponent();

            var viewModel = new ViewModels.MainWindowViewModel
            {
                RoiFormulas = roiFormulas,
                StructureDetails = structureDetails,
                StructureFormulas = roiFormulas.Formulas,
                DefaultDirectoryPath = defaultDirectoryPath
            };

            this.DataContext = viewModel;
        }
    }
}
