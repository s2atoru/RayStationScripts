using System.Collections.Generic;
using System.IO;
using System.Windows;
using System;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

using Juntendo.MedPhys;

namespace Juntendo.MedPhys.Esapi
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DvhObjectivesView : Window
    {
        public DvhObjectivesView(List<DvhObjective> objectives, PlanningItem planningItem, StructureSet structureSet)
        {
            InitializeComponent();

            var objectivesViewModel = new DvhObjectivesViewModel(objectives, planningItem, structureSet);

            this.DataContext = objectivesViewModel;
        }

        public DvhObjectivesView(Patient patient, PlanningItem planningItem, string folderPath)
        {
            InitializeComponent();

            var objectivesViewModel = new DvhObjectivesViewModel(patient, planningItem, folderPath);

            this.DataContext = objectivesViewModel;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            var ObjectivesVM = (DvhObjectivesViewModel)this.DataContext;
            var folderPath = ObjectivesVM.FolderPath;
            var objectives = new List<DvhObjective>();
            foreach (var o in ObjectivesVM.Objectives)
            {
                objectives.Add(o.Objective);
            }

            string patientId = ObjectivesVM.Patient.Id;
            string patientName = ObjectivesVM.Patient.LastName + " " + ObjectivesVM.Patient.FirstName.ToUpper();
            string courseId = ObjectivesVM.CourseId;
            string planId = ObjectivesVM.PlanId;

            double prescribedDose = ObjectivesVM.AssignedTotalPrescribedDoseTps;
            double maxDose = ObjectivesVM.MaxDose;
            if (ObjectivesVM.SystemDoseUnit == DoseValue.DoseUnit.cGy)
            {
                prescribedDose /= 100;
                maxDose /= 100;
            }

            string planFolderPath = Path.Combine(ObjectivesVM.FolderPath, Path.Combine(patientId, Path.Combine(planId, "PlanCheckData")));
            if (!Directory.Exists(planFolderPath))
            {
                Directory.CreateDirectory(planFolderPath);
            }

            var planInfo = new PlanInfo() { PatientId = patientId, PatientName = patientName, CourseId = courseId, PlanId = planId,
            PrescribedDose = prescribedDose, MaxDose = maxDose};

            if (objectives.Count == 0)
            {
                throw new InvalidOperationException("No item in objectives");
            }
            var originalProtocolId = objectives[0].OriginalProtocolId;

            DvhObjective.WriteObjectivesToFile(objectives, $"Last Saved ({patientId} {courseId} - {planId})", Path.Combine(planFolderPath, "LastSaved_sjis.csv"), planInfo);
            DvhObjective.WriteObjectivesToFile(objectives, originalProtocolId, Path.Combine(planFolderPath, "DvhInfo_sjis.csv"), planInfo);

            this.Close();
        }
    }
}
