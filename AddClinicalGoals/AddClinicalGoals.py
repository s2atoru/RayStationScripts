from connect import *

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

from Helpers import GetStructureSet
from Helpers import GetRoiDetails

case = get_current("Case")
examination = get_current("Examination")
structureSet = GetStructureSet(case, examination)
roiDetails = GetRoiDetails(structureSet)

#for key, value in roiDetails.items():
#    print key, value['HasContours']

plan = get_current("Plan")

clinicalGoalViewModel = ClinicalGoalViewModel()

dvhCheckerDirectoryPath = r"\\10.208.223.10\Eclipse\DvhChecker"
clinicalGoalViewModel.DvhCheckerDirectoryPath = dvhCheckerDirectoryPath

structureNames = List[str]()
for roiName in roiDetails.keys():
    structureNames.Add(roiName)

#structureNames = List[str]({'PTV', 'CTV', 'Rectal outline', 'Bladder outline'})

clinicalGoalViewModel.StructureNames = ObservableCollection[str](structureNames)

mainWindow = MainWindow(clinicalGoalViewModel)

mainWindow.ShowDialog()

if not clinicalGoalViewModel.CanExecute:
    print 'Canceled'
    sys.exit()

dvhObjectives = clinicalGoalViewModel.DvhObjectives;
prescribedDose = clinicalGoalViewModel.PrescribedDose;

from Helpers import AddClinicalGoal

with CompositeAction('Add Clinical Goals'):
    for dvhObjective in dvhObjectives:
        if len(dvhObjective.StructureNameTps) > 0 and dvhObjective.InUse:
            clinicalGoal = ClinicalGoal(dvhObjective, prescribedDose=prescribedDose)
            print clinicalGoal
            AddClinicalGoal(plan, clinicalGoal)
  # CompositeAction ends   
pass
