using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.IO;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

using Juntendo.MedPhys.Esapi;

namespace VMS.TPS
{
    public class Script
    {
        public Script()
        {
        }

        public void Execute(ScriptContext context /*, System.Windows.Window window*/)
        {
            Patient currentPatient = context.Patient;
            PlanSetup currentPlanSetup = context.PlanSetup;

            var folderPath = @"\\10.208.223.10\Eclipse";

            // For Non-clinical Eclipse
            var computerName = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
            var homePath = System.Environment.GetEnvironmentVariable("HOMEPATH");
            if (computerName == "ECQ275" || computerName == "ECM516NC" || computerName == "XPS13")
            {
                folderPath = Path.Combine(homePath, @"Desktop\PlanCheck");
            }

            var selectedPlanningItem = (PlanningItem)currentPlanSetup;
            var window = new DvhObjectivesView(currentPatient, selectedPlanningItem, folderPath);

            window.ShowDialog();
        }
    }
}
