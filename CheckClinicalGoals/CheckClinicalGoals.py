#from connect import *

import clr
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")
import sys, os

from System.Windows import MessageBox
from System.Collections.Generic import List, Dictionary
from System.Collections.ObjectModel import ObservableCollection

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

clr.AddReference("ClinicalGoal")
from ClinicalGoal.Views import MainWindow
from ClinicalGoal.ViewModels import ClinicalGoalViewModel
from ClinicalGoal.Models import ClinicalGoal

clr.AddReference("DvhChecker")
from Juntendo.MedPhys import DvhObjectiveType
from Juntendo.MedPhys import DvhTargetType
from Juntendo.MedPhys import DvhPresentationType
from Juntendo.MedPhys import DvhDoseUnit
from Juntendo.MedPhys import DvhVolumeUnit
from Juntendo.MedPhys import DvhEvalResult

from Helpers import GetStructureSet
from Helpers import GetRoiDetails
from Helpers import CheckDvhIndex

#case = get_current("Case")
#examination = get_current("Examination")
#plan = get_current("Plan")
#structureSet = GetStructureSet(case, examination)
#roiDetails = GetRoiDetails(structureSet)
#structureNames = List[str]()
#for roiName in roiDetails.keys():
#    structureNames.Add(roiName)
#structureNames = List[str]({'PTV', 'CTV', 'Rectal outline', 'Bladder outline'})

#dvhCheckerDirectoryPath = r"\\10.208.223.10\Eclipse\DvhChecker"
dvhCheckerDirectoryPath = RayStationScriptsPath + r"DvhChecker"

clinicalGoalViewModel = ClinicalGoalViewModel()
clinicalGoalViewModel.DvhCheckerDirectoryPath = dvhCheckerDirectoryPath

clinicalGoalViewModel.PlanCheckerDirectoryPath = r"\\10.208.223.10\Eclipse"

clinicalGoalViewModel.PatientId = "1234567890"
clinicalGoalViewModel.PatientName = "JUNTEN Taro"
clinicalGoalViewModel.CourseId = "C1"
clinicalGoalViewModel.PlanId = "1-1-1"

#for roiName in roiDetails.keys():
#    structureNames.Add(roiName)
structureNames = List[str]({'PTV', 'CTV', 'Rectal outline', 'Bladder outline'})

mainWindow = MainWindow(clinicalGoalViewModel)
mainWindow.Title = "Check Clinical Goal"

mainWindow.ShowDialog()

if not clinicalGoalViewModel.CanExecute:
    print 'Canceled'
    sys.exit()

dvhObjectives = clinicalGoalViewModel.DvhObjectives;
prescrivedDose = clinicalGoalViewModel.PrescribedDose;

mainWindow = MainWindow(clinicalGoalViewModel)
mainWindow.Title = "Save Clinical Goal"
mainWindow.ShowDialog()

for o in dvhObjectives:
    print o.TargetType, o.DoseUnit, o.VolumeUnit, o.ObjectiveType
    if (o.TargetType == DvhTargetType.Volume):
        print "Dose"
    CheckDvhIndex(o)
pass