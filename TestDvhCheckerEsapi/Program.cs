using Juntendo.MedPhys.Esapi;
using System;
using System.IO;

using VMS.TPS.Common.Model.API;

namespace TestDvhCheckerEsapi
{
    namespace Juntendo.MedPhys.Esapi
    {
        class Program
        {
            [STAThread]
            static void Main(string[] args)
            {
                try
                {
                    using (var app = VMS.TPS.Common.Model.API.Application.CreateApplication("SysAdmin", "SysAdmin"))
                    {
                        Execute(app);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                    Console.ReadLine();
                }
            }
            static void Execute(VMS.TPS.Common.Model.API.Application app)
            {

                var patientId = "Br0001";
                var courseId = "C1";
                var planId = "FF_Open1";

                var PatientId = patientId;

                var selectedPatient = app.OpenPatientById(patientId);

                PlanningItem selectedPlanningItem;
                StructureSet selectedStructureSet;
                var selectedCourse = EsapiHelpers.GetCourse(selectedPatient, courseId);
                var selectedPlanSetup = EsapiHelpers.GetPlanSetup(selectedCourse, planId);
                PlanSum selectedPlanSum;
                if (selectedPlanSetup != null)
                {
                    selectedPlanningItem = (PlanningItem)selectedPlanSetup;
                    selectedStructureSet = selectedPlanSetup.StructureSet;

                }
                else
                {
                    selectedPlanSum = EsapiHelpers.GetPlanSum(selectedCourse, planId);
                    if (selectedPlanSum == null)
                    {
                        throw new InvalidOperationException($"No corresponding plan: {planId}");
                    }
                    selectedPlanningItem = (PlanningItem)selectedPlanSum;
                    selectedStructureSet = selectedPlanSum.StructureSet;
                }

                var CourseId = courseId;
                var PlanId = planId;

                var folderPath = @"\\10.208.223.10\Eclipse";

                // For Non-clinical Eclipse
                var computerName = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
                var homePath = System.Environment.GetEnvironmentVariable("HOMEPATH");
                if (computerName == "ECQ275" || computerName == "ECM516NC" || computerName == "XPS13")
                {
                    folderPath = Path.Combine(homePath, @"Desktop\PlanCheck");
                }

                //var dvhTemplatesPath = Path.Combine(folderPath, @"templates");
                //var files = Directory.GetFiles(dvhTemplatesPath);

                ////var homePath = Environment.GetEnvironmentVariable("HOMEPATH");
                //string filePath = Path.Combine(folderPath, @"templates\DVHObjectives.csv");

                //var objectives = DvhObjective.ReadObjectivesFromCsv(filePath);

                //var window = new DvhObjectivesView(objectives, selectedPlanningItem, selectedStructureSet);

                //var window = new DvhObjectivesView(selectedPatient, selectedPlanningItem, folderPath);

                var window = new DvhExporterView(selectedPatient, selectedPlanningItem, folderPath);

                window.ShowDialog();
            }
        }
    }
}
