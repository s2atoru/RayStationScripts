using Prism.Commands;
using Microsoft.Win32;
using System.IO;
using Juntendo.MedPhys;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClinicalGoal.ViewModels
{
    public class ClinicalGoalViewModel : BindableBaseWithErrorsContainer
    {

        public string DvhCheckerDirectoryPath { get; set; } = string.Empty;

        public bool CanExecute { get; private set; } = false;

        public int PrescribedDose { get; set; } = 0;

        private string protocolId;
        public string ProtocolId
        {
            get { return protocolId; }
            set { SetProperty(ref protocolId, value); }
        }

        private string protocolFilePath = string.Empty;
        public string ProtocolFilePath
        {
            get { return protocolFilePath; }
            set { SetProperty(ref protocolFilePath, value); }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private ObservableCollection<DvhObjective> dvhObjectives = new ObservableCollection<DvhObjective>();
        public ObservableCollection<DvhObjective> DvhObjectives
        {
            get { return dvhObjectives; }
            set { SetProperty(ref dvhObjectives, value); }
        }

        private ObservableCollection<string> structureNames = new ObservableCollection<string>{"PTV", "CTV", "Rectal wall", "Bladder wall"};
        public ObservableCollection<string> StructureNames
        {
            get { return structureNames; }
            set { SetProperty(ref structureNames, value); }
        }

        public DelegateCommand ChooseFileCommand { get; private set; }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public ClinicalGoalViewModel()
        {
            ChooseFileCommand = new DelegateCommand(ChooseFile);
            OkCommand = new DelegateCommand(() => { CanExecute = true; });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
        }

        private void ChooseFile()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Choose file",
                Filter = "CSV file (*.csv)|*.csv",
                InitialDirectory = Path.Combine(DvhCheckerDirectoryPath, "templates")
            };
            if (dialog.ShowDialog() == true)
            {
                ProtocolFilePath = dialog.FileName;
            }
            else
            {
                ProtocolFilePath = string.Empty;
                Message = "\"Choose file\" is canceled";
                return;
            }

            DvhObjectives = new ObservableCollection<DvhObjective>(DvhObjective.ReadObjectivesFromCsv(ProtocolFilePath));

            foreach (var o in DvhObjectives)
            {
                if (StructureNames.Contains(o.StructureName))
                {
                    o.StructureNameTps = o.StructureName;
                    o.InUse = true;
                }
                else
                {
                    o.StructureNameTps = string.Empty;
                }
            }

            if (DvhObjectives.Count > 0)
            {
                ProtocolId = DvhObjectives[0].ProtocolId;
            }
        }
    }
}
