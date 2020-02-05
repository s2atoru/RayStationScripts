using Prism.Commands;
using Microsoft.Win32;
using System.IO;
using Juntendo.MedPhys;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ClinicalGoal.ViewModels
{
    public class ClinicalGoalViewModel : BindableBaseWithErrorsContainer
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }

        private string planId;
        public string PlanId
        {
            get { return planId; }
            set { SetProperty(ref planId, value); }
        }

        private string courseId;
        public string CourseId
        {
            get { return courseId; }
            set { SetProperty(ref courseId, value); }
        }

        private double maxDose;
        public double MaxDose
        {
            get { return maxDose; }
            set { SetProperty(ref maxDose, value); }
        }

        public string PlanCheckerDirectoryPath { get; set; } = string.Empty;

        public string DvhCheckerDirectoryPath { get; set; } = string.Empty;

        public bool CanExecute { get; private set; } = false;

        public int PrescribedDose { get; set; } = 0;

        private bool clearAllExistingClinicalGoals;
        public bool ClearAllExistingClinicalGoals
        {
            get { return clearAllExistingClinicalGoals; }
            set { SetProperty(ref clearAllExistingClinicalGoals, value); }
        }

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

        private ObservableCollection<string> structureNames = new ObservableCollection<string> { "PTV", "CTV", "Rectal wall", "Bladder wall" };
        public ObservableCollection<string> StructureNames
        {
            get { return structureNames; }
            set { SetProperty(ref structureNames, value); }
        }

        public DelegateCommand ChooseFileCommand { get; private set; }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand SaveDvhIndicesCommand { get; private set; }

        public ClinicalGoalViewModel()
        {
            ChooseFileCommand = new DelegateCommand(ChooseFile);
            OkCommand = new DelegateCommand(() => { CanExecute = true; PickUpDvhObjectivesInUse(); });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
            SaveDvhIndicesCommand = new DelegateCommand(() => { SaveDvhIndices(); });
        }

        private void SaveDvhIndices()
        {
            var planInfo = new PlanInfo
            {
                PatientId = PatientId,
                PatientName = PatientName,
                CourseId = CourseId,
                PlanId = PlanId,
                // cGy to Gy
                PrescribedDose = PrescribedDose / 100,
                MaxDose = MaxDose
            };

            string planFolderPath = Path.Combine(PlanCheckerDirectoryPath, Path.Combine(PatientId, Path.Combine(PlanId, "PlanCheckData")));
            if (!Directory.Exists(planFolderPath))
            {
                Directory.CreateDirectory(planFolderPath);
            }

            if (DvhObjectives.Count > 0)
            {
                var originalProtocolId = DvhObjectives[0].OriginalProtocolId;
                DvhObjective.WriteObjectivesToFile(DvhObjectives.ToList(), originalProtocolId, Path.Combine(planFolderPath, "DvhInfo_sjis.csv"), planInfo);
            }
            else
            {
                MessageBox.Show("No Objective");
            }

            return;
        }

        private void PickUpDvhObjectivesInUse()
        {
            var dvhObjectivesInUse = DvhObjectives.Where(o => (o.InUse && o.StructureNameTps.Length > 0));
            DvhObjectives = new ObservableCollection<DvhObjective>(dvhObjectivesInUse);
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
