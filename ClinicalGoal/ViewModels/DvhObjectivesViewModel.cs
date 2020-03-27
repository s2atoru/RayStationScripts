using Juntendo.MedPhys;
using MvvmCommon.ViewModels;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ClinicalGoal.ViewModels
{
    public class DvhObjectivesViewModel : BindableBaseWithErrorsContainer
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

        private double prescribedDose = 0;

        [CustomValidation(typeof(DvhObjectivesViewModel), "IsPositiveDouble", ErrorMessage = "Enter > 0")]
        public double PrescribedDose
        {
            get { return prescribedDose; }
            set { SetProperty(ref prescribedDose, value); }
        }
        public static ValidationResult IsPositiveDouble(double value, ValidationContext context)
        {
            if (value <= 0)
            {
                return new ValidationResult(null);
            }
            return ValidationResult.Success;
        }

        private int numberOfFractions = 1;
        public int NumberOfFractions
        {
            get { return numberOfFractions; }
            set { SetProperty(ref numberOfFractions, value); }
        }

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
