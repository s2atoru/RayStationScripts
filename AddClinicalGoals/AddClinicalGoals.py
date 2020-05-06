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
print 'DLL path: ', dllsPath
sys.path.append(dllsPath)

#scriptsPath = RayStationScriptsPath + "Scripts"
#print(scriptsPath)
#sys.path.append(scriptsPath)

RayStationScriptsCommonPath = r"\\10.208.223.10\RayStation\RayStationScripts" + "\\"
scriptsPath = RayStationScriptsCommonPath + "Scripts"
print 'Script path: ', scriptsPath
sys.path.append(scriptsPath)


clr.AddReference("ClinicalGoal")
from ClinicalGoal.Views import MainWindow
from ClinicalGoal.ViewModels import ClinicalGoalViewModel
from ClinicalGoal.Models import ClinicalGoal
from ClinicalGoal.Models import PlanPrescription

from Helpers import GetStructureSet
from Helpers import GetRoiDetails
from Helpers import SizeOfIterator

patient = get_current("Patient")
case = get_current("Case")
examination = get_current("Examination")
structureSet = GetStructureSet(case, examination)
roiDetails = GetRoiDetails(structureSet)

#for key, value in roiDetails.items():
#    print key, value['HasContours']

plan = get_current("Plan")

dvhCheckerDirectoryPath = r"\\10.208.223.10\Eclipse\DvhChecker"
planCheckerDirectoryPath = r"\\10.208.223.10\Eclipse"

structureNames = List[str]()
for roiName in roiDetails.keys():
    structureNames.Add(roiName)

#planPrescriptions = List[PlanPrescription]()
#beamSet = get_current('BeamSet')
#planPrescription = PlanPrescription()
#planPrescription.PlanId = beamSet.DicomPlanLabel
#planPrescription.PrescribedDose = beamSet.Prescription.DosePrescriptions[0].DoseValue
#planPrescription.NumberOfFractions = beamSet.FractionationPattern.NumberOfFractions
#planPrescriptions.Add(planPrescription)

planPrescriptions = List[PlanPrescription]()
totalDose = 0
for beamSet in plan.BeamSets:
    planPrescription = PlanPrescription()
    planPrescription.PlanId = beamSet.DicomPlanLabel
    planPrescription.PrescribedDose = beamSet.Prescription.DosePrescriptions[0].DoseValue
    totalDose += planPrescription.PrescribedDose
    planPrescription.NumberOfFractions = beamSet.FractionationPattern.NumberOfFractions
    planPrescriptions.Add(planPrescription)

if SizeOfIterator(plan.BeamSets) >= 2:
    planPrescription = PlanPrescription()
    planPrescription.PlanId = "Combined dose"
    planPrescription.PrescribedDose = totalDose
    planPrescription.NumberOfFractions = 1
    planPrescriptions.Add(planPrescription)

patientId = patient.PatientID
patientNameDicom = patient.PatientName

clinicalGoalViewModel = ClinicalGoalViewModel(patientId, patientNameDicom, planPrescriptions, structureNames, dvhCheckerDirectoryPath, planCheckerDirectoryPath)
clinicalGoalViewModel.ClearAllExistingClinicalGoals = True

beamSet = get_current('BeamSet')
clinicalGoalViewModel.SelectDvhObjectivesViewModel(beamSet.DicomPlanLabel)

mainWindow = MainWindow(clinicalGoalViewModel)

mainWindow.ShowDialog()

if not clinicalGoalViewModel.CanExecute:
    print 'Canceled'
    sys.exit()

dvhObjectivesViewModel = clinicalGoalViewModel.DvhObjectivesViewModels[0]
dvhObjectives = dvhObjectivesViewModel.DvhObjectives
prscrivedDose = dvhObjectivesViewModel.PrescribedDose 
clearAllExistingClinicalGoals = clinicalGoalViewModel.ClearAllExistingClinicalGoals

from Helpers import AddClinicalGoal, ClearAllClinicalGoals

if clearAllExistingClinicalGoals:
    ClearAllClinicalGoals(plan)
    
with CompositeAction('Add Clinical Goals'):
    for dvhObjective in dvhObjectives:
        if len(dvhObjective.StructureNameTps) > 0 and dvhObjective.InUse:
            clinicalGoal = ClinicalGoal(dvhObjective, prescribedDose=prscrivedDose)
            print clinicalGoal
            AddClinicalGoal(plan, clinicalGoal)
  # CompositeAction ends   
pass
