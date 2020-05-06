from connect import *

import clr
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")
import sys, os

from System.Windows import MessageBox
from System.Collections.Generic import List, Dictionary
from System.Collections.ObjectModel import ObservableCollection

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\Desktop\RayStationScripts" + "\\"

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
from ClinicalGoal.Views import MultiplePlansWindow
from ClinicalGoal.ViewModels import ClinicalGoalViewModel
from ClinicalGoal.Models import ClinicalGoal
from ClinicalGoal.Models import PlanPrescription

from Helpers import GetStructureSet
from Helpers import GetRoiDetails
from Helpers import CheckDvhIndex
from Helpers import SizeOfIterator

patient = get_current("Patient")
case = get_current("Case")
examination = get_current("Examination")
plan = get_current("Plan")

structureSet = GetStructureSet(case, examination)
roiDetails = GetRoiDetails(structureSet)
structureNames = List[str]()
for roiName in roiDetails.keys():
    if roiDetails[roiName]['HasContours']:
        structureNames.Add(roiName)

#structureNames = List[str]({'PTV', 'CTV', 'Rectal outline', 'Bladder outline'})

dvhCheckerDirectoryPath = r"\\10.208.223.10\Eclipse\DvhChecker"
planCheckerDirectoryPath = r"\\10.208.223.10\Eclipse"
#dvhCheckerDirectoryPath = RayStationScriptsPath + r"DvhChecker"

clinicalGoalViewModel = ClinicalGoalViewModel()
#clinicalGoalViewModel.DvhCheckerDirectoryPath = dvhCheckerDirectoryPath
#clinicalGoalViewModel.PlanCheckerDirectoryPath = r"\\10.208.223.10\Eclipse"

patientId = patient.PatientID
patientNameDicom = patient.PatientName

planPrescriptions = List[PlanPrescription]()
totalDose = 0
for beamSet in plan.BeamSets:
    planPrescription = PlanPrescription()
    planPrescription.PlanId = beamSet.DicomPlanLabel
    planPrescription.PrescribedDose = beamSet.Prescription.DosePrescriptions[0].DoseValue
    totalDose += planPrescription.PrescribedDose
    planPrescription.NumberOfFractions = beamSet.FractionationPattern.NumberOfFractions
    planPrescriptions.Add(planPrescription)

#po = GetPlanOptimizationForBeamSet(plan.PlanOptimizations, beamSet)
#if SizeOfIterator(po.OptimizedBeamSets) == 2 or SizeOfIterator(plan.BeamSets) >= 2:
if SizeOfIterator(plan.BeamSets) >= 2:
    #hasMultipleBeamSets = True
    planPrescription = PlanPrescription()
    planPrescription.PlanId = "Combined dose"
    planPrescription.PrescribedDose = totalDose
    planPrescription.NumberOfFractions = 1
    planPrescriptions.Add(planPrescription)
#else:
#    hasMultipleBeamSets = False

clinicalGoalViewModel = ClinicalGoalViewModel(patientId, patientNameDicom, planPrescriptions, structureNames, dvhCheckerDirectoryPath, planCheckerDirectoryPath)
clinicalGoalViewModel.ClearAllExistingClinicalGoals = True
clinicalGoalViewModel.DoesSetDoseToAbs = True
clinicalGoalViewModel.SlackForDvhCheck = 1.e-2

mainWindow = MultiplePlansWindow(clinicalGoalViewModel)
mainWindow.Title = "Check Clinical Goal"
mainWindow.ShowDialog()

if not clinicalGoalViewModel.CanExecute:
    print 'Canceled'
    sys.exit()

slack = clinicalGoalViewModel.SlackForDvhCheck
print slack
for vm in clinicalGoalViewModel.DvhObjectivesViewModels:
    
    planId = vm.PlanId
    numberOfFractions = vm.NumberOfFractions
    prescribedDose = vm.PrescribedDose
    dvhObjectives = vm.DvhObjectives

    if planId == "Combined dose":
        planDose = plan.TreatmentCourse.TotalDose
        numberOfFractions = 1
    else:
        bs = plan.BeamSets[planId]
        planDose = bs.FractionDose

    #planDose = beamSet.FractionDose
    #dvhObjectives = clinicalGoalViewModel.DvhObjectives
    #prescrivedDose = clinicalGoalViewModel.PrescribedDose

    for o in dvhObjectives:
        print o.StructureName, o.StructureNameTps, o.TargetType, o.DoseUnit, o.VolumeUnit, o.ObjectiveType
        print o.ArgumentValue, o.TargetValue
    #    if (o.TargetType == DvhTargetType.Volume):
    #        print "Dose"
        CheckDvhIndex(o, prescribedDose, roiDetails, planDose, numberOfFractions, slack, False)
        print o.Value, o.IsPassed, o.IsAcceptable

#pass

clinicalGoalViewModel.IsSaving = True
mainWindow = MultiplePlansWindow(clinicalGoalViewModel)
mainWindow.Title = "Save Clinical Goal"
mainWindow.ShowDialog()
