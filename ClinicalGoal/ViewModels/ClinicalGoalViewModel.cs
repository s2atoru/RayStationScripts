using Juntendo.MedPhys;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ClinicalGoal.ViewModels
{
    public class ClinicalGoalViewModel : BindableBase
    {
        private static readonly string StructureNameNone = "(none)";
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public List<string> PlanIds { get; private set; }

        private string selectedPlanId;
        public string SelectedPlanId
        {
            get { return selectedPlanId; }
            set { SetProperty(ref selectedPlanId, value); }
        }

        public string PlanCheckerDirectoryPath { get; set; } = string.Empty;

        public string DvhCheckerDirectoryPath { get; set; } = string.Empty;

        public bool CanExecute { get; private set; } = false;

        private bool clearAllExistingClinicalGoals;
        public bool ClearAllExistingClinicalGoals
        {
            get { return clearAllExistingClinicalGoals; }
            set { SetProperty(ref clearAllExistingClinicalGoals, value); }
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

        private ObservableCollection<string> structureNames = new ObservableCollection<string> { "PTV", "CTV", "Rectum", "Bladder" };
        public ObservableCollection<string> StructureNames
        {
            get { return structureNames; }
            set { SetProperty(ref structureNames, value); }
        }

        private DvhObjectivesViewModel selectedDvhObjectivesViewModel;
        public DvhObjectivesViewModel SelectedDvhObjectivesViewModel
        {
            get { return selectedDvhObjectivesViewModel; }
            set { SetProperty(ref selectedDvhObjectivesViewModel, value); }
        }

        private ObservableCollection<DvhObjectivesViewModel> dvhObjectivesViewModels = new ObservableCollection<DvhObjectivesViewModel>();
        public ObservableCollection<DvhObjectivesViewModel> DvhObjectivesViewModels
        {
            get { return dvhObjectivesViewModels; }
            set { SetProperty(ref dvhObjectivesViewModels, value); }
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand SaveDvhIndicesCommand { get; private set; }

        public ClinicalGoalViewModel()
        {
            PatientId = "TESTID";
            PatientName = "TEST^TEST";

            DvhCheckerDirectoryPath = @"C:\Users\s2ato\Desktop\RayStationScripts\DvhChecker";
            PlanCheckerDirectoryPath = @"C:\Users\s2ato\Desktop\RayStationScripts\DvhChecker";

            OkCommand = new DelegateCommand(() => { CanExecute = true; PickUpDvhObjectivesInUse(); });
            CancelCommand = new DelegateCommand(() => { CanExecute = false; });
            SaveDvhIndicesCommand = new DelegateCommand(() => { SaveDvhIndices(); });

            var planPrescriptions = new List<Models.PlanPrescription>();
            planPrescriptions.Add(new Models.PlanPrescription { PlanId = "1-1-1", PrescribedDose = 4600, NumberOfFractions = 23 });
            planPrescriptions.Add(new Models.PlanPrescription { PlanId = "1-1-2", PrescribedDose = 3000, NumberOfFractions = 15 });
            planPrescriptions.Add(new Models.PlanPrescription { PlanId = "Plan Sum", PrescribedDose = 7600, NumberOfFractions = 38 });

            foreach (var p in planPrescriptions)
            {
                var dvhObjectivesViewModel = new DvhObjectivesViewModel
                {
                    NumberOfFractions = p.NumberOfFractions,
                    PlanId = p.PlanId,
                    PrescribedDose = p.PrescribedDose
                };

                dvhObjectivesViewModel.ChooseFileCommand = new DelegateCommand<DvhObjectivesViewModel>(ChooseFile);
                dvhObjectivesViewModel.StructureNames = StructureNames;

                DvhObjectivesViewModels.Add(dvhObjectivesViewModel);
            }


            if(DvhObjectivesViewModels.Count > 0)
            {
                SelectedDvhObjectivesViewModel = DvhObjectivesViewModels.First();
            }
        }

        private void SaveDvhIndices()
        {
            foreach (var dvhObjectivesViewModel in DvhObjectivesViewModels)
            {
                var planInfo = new PlanInfo
                {
                    PatientId = PatientId,
                    PatientName = PatientName,
                    CourseId = dvhObjectivesViewModel.CourseId,
                    PlanId = dvhObjectivesViewModel.PlanId,
                    // cGy to Gy
                    PrescribedDose = dvhObjectivesViewModel.PrescribedDose / 100,
                    MaxDose = dvhObjectivesViewModel.MaxDose
                };

                string planFolderPath = Path.Combine(PlanCheckerDirectoryPath, Path.Combine(PatientId, Path.Combine(planInfo.PlanId, "PlanCheckData")));
                if (!Directory.Exists(planFolderPath))
                {
                    Directory.CreateDirectory(planFolderPath);
                }

                var dvhObjectives = dvhObjectivesViewModel.DvhObjectives;
                if (dvhObjectives.Count > 0)
                {
                    var originalProtocolId = dvhObjectives[0].OriginalProtocolId;
                    DvhObjective.WriteObjectivesToFile(dvhObjectives.ToList(), originalProtocolId, Path.Combine(planFolderPath, "DvhInfo_sjis.csv"), planInfo);
                }
                else
                {
                    MessageBox.Show("No Objective");
                }
            }

            return;
        }

        private void PickUpDvhObjectivesInUse()
        {
            foreach (var dvhObjectivesViewModel in DvhObjectivesViewModels)
            {
                var dvhObjectivesInUse = dvhObjectivesViewModel.DvhObjectives.Where(o => (o.InUse && o.StructureNameTps.Length > 0));
                dvhObjectivesViewModel.DvhObjectives = new ObservableCollection<DvhObjective>(dvhObjectivesInUse);

                var planId = dvhObjectivesViewModel.PlanId;
                string planFolderPath = Path.Combine(PlanCheckerDirectoryPath, Path.Combine(PatientId, Path.Combine(planId, "PlanCheckData")));
                if (!Directory.Exists(planFolderPath))
                {
                    Directory.CreateDirectory(planFolderPath);
                }
                var filePath = Path.Combine(planFolderPath, "DvhObjectives.json");

                string json = JsonConvert.SerializeObject(dvhObjectivesInUse.ToList(), Formatting.Indented);
                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("shift_jis")))
                {
                    sw.Write(json);
                }
            }
        }
        private void ChooseFile(DvhObjectivesViewModel dvhObjectivesViewModel)
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

            dvhObjectivesViewModel.DvhObjectives = new ObservableCollection<DvhObjective>(DvhObjective.ReadObjectivesFromCsv(ProtocolFilePath));

            var dvhObjectives = dvhObjectivesViewModel.DvhObjectives;
            if (!StructureNames.Contains(StructureNameNone)) StructureNames.Add(StructureNameNone);
            foreach (var o in dvhObjectives)
            {
                if (StructureNames.Contains(o.StructureName))
                {
                    o.StructureNameTps = o.StructureName;
                    o.InUse = true;
                }
                else
                {
                    o.StructureNameTps = StructureNameNone;
                }
            }

            if (dvhObjectives.Count > 0)
            {
                dvhObjectivesViewModel.ProtocolId = dvhObjectives[0].ProtocolId;
            }
        }
    }
}
