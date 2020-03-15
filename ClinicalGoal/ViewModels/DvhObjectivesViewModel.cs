using Juntendo.MedPhys;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ClinicalGoal.ViewModels
{
    public class DvhObjectivesViewModel : BindableBase
    {
        static readonly string PlanIdPlanSum = "Plan Sum";
        static readonly string CourseIdNone = "NA";

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string planId;
        public string PlanId
        {
            get { return planId; }
            set { SetProperty(ref planId, value); }
        }

        private string courseId = CourseIdNone;
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

        public double PrescribedDose { get; set; } = 0;

        public int NumberOfFractions { get; set; } = 1;

        private string protocolId;
        public string ProtocolId
        {
            get { return protocolId; }
            set { SetProperty(ref protocolId, value); }
        }

        private ObservableCollection<string> structureNames = new ObservableCollection<string>();
        public ObservableCollection<string> StructureNames
        {
            get { return structureNames; }
            set { SetProperty(ref structureNames, value); }
        }

        private ObservableCollection<DvhObjective> dvhObjectives = new ObservableCollection<DvhObjective>();
        public ObservableCollection<DvhObjective> DvhObjectives
        {
            get { return dvhObjectives; }
            set { SetProperty(ref dvhObjectives, value); }
        }

        public DelegateCommand<DvhObjectivesViewModel> ChooseFileCommand { get; set; }

        //public string PlanCheckerDirectoryPath { get; set; } = string.Empty;

        //private string protocolFilePath = string.Empty;
        //public string ProtocolFilePath
        //{
        //    get { return protocolFilePath; }
        //    set { SetProperty(ref protocolFilePath, value); }
        //}
        //public string DvhCheckerDirectoryPath { get; set; } = string.Empty;

        //private ObservableCollection<string> structureNames = new ObservableCollection<string> { "PTV", "CTV", "Rectal wall", "Bladder wall" };
        //public ObservableCollection<string> StructureNames
        //{
        //    get { return structureNames; }
        //    set { SetProperty(ref structureNames, value); }
        //}

        //private void ChooseFile(DvhObjectivesViewModel dvhObjectivesViewModel)
        //{
        //    var dialog = new OpenFileDialog
        //    {
        //        Title = "Choose file",
        //        Filter = "CSV file (*.csv)|*.csv",
        //        InitialDirectory = Path.Combine(DvhCheckerDirectoryPath, "templates")
        //    };
        //    if (dialog.ShowDialog() == true)
        //    {
        //        ProtocolFilePath = dialog.FileName;
        //    }
        //    else
        //    {
        //        ProtocolFilePath = string.Empty;
        //        Message = "\"Choose file\" is canceled";
        //        return;
        //    }

        //    dvhObjectivesViewModel.DvhObjectives = new ObservableCollection<DvhObjective>(DvhObjective.ReadObjectivesFromCsv(ProtocolFilePath));

        //    var dvhObjectives = dvhObjectivesViewModel.DvhObjectives;
        //    foreach (var o in dvhObjectives)
        //    {
        //        if (StructureNames.Contains(o.StructureName))
        //        {
        //            o.StructureNameTps = o.StructureName;
        //            o.InUse = true;
        //        }
        //        else
        //        {
        //            o.StructureNameTps = string.Empty;
        //        }
        //    }

        //    if (dvhObjectives.Count > 0)
        //    {
        //        dvhObjectivesViewModel.ProtocolId = dvhObjectives[0].ProtocolId;
        //    }
        //}
    }
}
