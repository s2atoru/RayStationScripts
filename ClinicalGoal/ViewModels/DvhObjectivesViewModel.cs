using Juntendo.MedPhys;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace ClinicalGoal.ViewModels
{
    public class DvhObjectivesViewModel : BindableBase
    {
        static readonly string PlanIdPlanSum = "Plan Sum";
        static readonly string CourseIdNone = "NA";

        private bool isStructureNameTpsSynchronized = true;
        public bool IsStructureNameTpsSynchronized
        {
            get { return isStructureNameTpsSynchronized; }
            set { SetProperty(ref isStructureNameTpsSynchronized, value); }
        }

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

        public DelegateCommand<DvhObjective> SynchronizedStructureNameTpsCommand { get; private set; }

        public DelegateCommand<DvhObjectivesViewModel> ChooseFileCommand { get; set; }

        public DvhObjectivesViewModel()
        {
            SynchronizedStructureNameTpsCommand = new DelegateCommand<DvhObjective>(SynchronizedStructureNameTps);
        }

        private void SynchronizedStructureNameTps(DvhObjective dvhObjective)
        {
            if (!IsStructureNameTpsSynchronized) return;

            var dvhObjectivesWithSameStructureName = DvhObjectives.Where(d => ((d.StructureName == dvhObjective.StructureName) && (d.StructureNameTps != dvhObjective.StructureNameTps)));
            foreach (var d in dvhObjectivesWithSameStructureName)
            {
                 d.StructureNameTps = dvhObjective.StructureNameTps;
            }
        }
    }
}
