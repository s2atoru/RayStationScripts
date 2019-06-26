using System.Collections.Generic;
using System.IO;
using System.Windows;
using System;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

using Juntendo.MedPhys;
using System.Text;
using System.Linq;

namespace Juntendo.MedPhys.Esapi
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DvhExporterView : Window
    {
        public DvhExporterView(List<DvhObjective> objectives, PlanningItem planningItem, StructureSet structureSet)
        {
            InitializeComponent();

            var objectivesViewModel = new DvhObjectivesViewModel(objectives, planningItem, structureSet);

            this.DataContext = objectivesViewModel;
        }

        public DvhExporterView(Patient patient, PlanningItem planningItem, string folderPath)
        {
            InitializeComponent();

            var objectivesViewModel = new DvhObjectivesViewModel(patient, planningItem, folderPath);

            this.DataContext = objectivesViewModel;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            var objectivesVM = (DvhObjectivesViewModel)this.DataContext;
            var folderPath = objectivesVM.FolderPath;
            var objectives = new List<DvhObjective>();
            foreach (var o in objectivesVM.Objectives)
            {
                objectives.Add(o.Objective);
            }

            string patientId = objectivesVM.Patient.Id;
            string patientName = objectivesVM.Patient.LastName + " " + objectivesVM.Patient.FirstName.ToUpper();
            string courseId = objectivesVM.CourseId;
            string planId = objectivesVM.PlanId;

            double prescribedDose = objectivesVM.AssignedTotalPrescribedDoseTps;
            double maxDose = objectivesVM.MaxDose;
            if (objectivesVM.SystemDoseUnit == DoseValue.DoseUnit.cGy)
            {
                prescribedDose /= 100;
                maxDose /= 100;
            }

            string planFolderPath = Path.Combine(objectivesVM.FolderPath, Path.Combine(patientId, Path.Combine(planId, "PlanCheckData")));
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

            var dvhProtocol = new DvhProtocol(objectivesVM);

            List<string> titles = dvhProtocol.GetObjectiveTitles();
            var dvhRecord = dvhProtocol.GetDvhIndexCsvRecord(titles);

            var structureNames = new List<string>();

            foreach (var objective in objectives)
            {
                var structureName = objective.StructureName;
                if (!structureNames.Contains(structureName))
                {
                    structureNames.Add(structureName);
                }
            }

            string headerStructureVolumes = String.Empty;
            string volumes = string.Empty;

            foreach (var structureName in structureNames)
            {
                headerStructureVolumes += "," + DvhProtocol.Quoted(structureName + " volume");

                var query = from o in objectives where o.StructureName == structureName select o;

                var volume = query.First().Volume;

                volumes += "," + DvhProtocol.Quoted(volume);
            }

            string outputFileName = "DvhSummary.csv";
            string outputFilePath = Path.Combine(folderPath, outputFileName);

            bool headerFlag = true;
            bool appendFlag = false;

            if (File.Exists(outputFilePath))
            {
                appendFlag = true;
                headerFlag = false;
            }

            string header = DvhProtocol.GetStringCsvRecord(new List<string> { "PatientId", "CourseId", "PlanId", "PrescribedDose" });
            var planInfoItems = new List<string> { patientId, courseId, planId, prescribedDose.ToString() };

            using (StreamWriter sw = new StreamWriter(outputFilePath, appendFlag, Encoding.GetEncoding("shift_jis")))
            {
                if (headerFlag == true)
                {
                    header += "," + DvhProtocol.GetStringCsvRecord(titles) + headerStructureVolumes;
                    sw.WriteLine(header);
                    headerFlag = false;
                }

                var records = DvhProtocol.GetStringCsvRecord(planInfoItems) + "," + dvhRecord + volumes;
                sw.WriteLine(records);
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DVH parameters are not written in the file");
            this.Close();
        }
    }
}
