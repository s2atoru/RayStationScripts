﻿using System.Windows;
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

        public MainWindow(List<Models.StructureDetail> structureDetails, string defaultTargetName = "PTV", string defaultDose100VolumeName = "zDose100^GI", string defaultDose50VolumeName = "zDose50^GI", string defaultOverlapTargetDose100VolumeName = "zTVPV^GI")
        {
            InitializeComponent();

            DataContext = new ViewModels.DoseIndicesViewModel(structureDetails, defaultTargetName, defaultDose100VolumeName, defaultOverlapTargetDose100VolumeName);
        }
    }
}
