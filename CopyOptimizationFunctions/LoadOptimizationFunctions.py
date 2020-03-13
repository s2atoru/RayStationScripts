from connect import *

import clr
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

from System.Collections.Generic import List, Dictionary
from System.Windows import MessageBox

import sys, os, codecs
import json
from collections import OrderedDict

dllsPath = os.environ["USERPROFILE"]
dllsPath = dllsPath + r"\Desktop\RayStationScripts\Dlls"
print(dllsPath)
sys.path.append(dllsPath)

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"
scriptsPath = RayStationScriptsPath + "Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

from Helpers import ClearObjectiveFunctions
from Helpers import GetObjectiveFunctionDescription
from Helpers import GetPlanLabelForConstituentFunction
from Helpers import IsCombinedConstituentFunction
from Helpers import SizeOfIterator

clr.AddReference("OptimizationFunctionCopyManager")
from OptimizationFunctionCopyManager.ViewModels import LoadObjectiveFunctionsViewModel
from OptimizationFunctionCopyManager.Views import LoadWindow
from OptimizationFunctionCopyManager.Models import Roi

objectiveFunctionsPath = r"\\10.208.223.10\RayStation\RayStationScripts\ObjectiveFunctions"

case = get_current("Case")
# Get handles to "Original plan" and "Copy plan".
plan = get_current('Plan')
beamSets = plan.BeamSets

from Helpers import GetStructureSet, GetRoiDetails
examination = get_current("Examination")
structureSet = GetStructureSet(case, examination)
roiDetails = GetRoiDetails(structureSet)

rois = List[Roi]()
for key, value in roiDetails.items():
    if value['HasContours']:
        rois.Add(Roi(key))
    
planLabels = List[str]()
for bs in beamSets:
    planLabels.Add(bs.DicomPlanLabel)

loadObjectiveFunctionsViewModel = LoadObjectiveFunctionsViewModel(rois, planLabels, objectiveFunctionsPath)
#loadWindow = LoadWindow(rois, planLabels, objectiveFunctionsPath)
loadWindow = LoadWindow()
loadWindow.DataContext = loadObjectiveFunctionsViewModel
loadWindow.ShowDialog()
if not loadObjectiveFunctionsViewModel.CanExecute:
    print "Canceled"
    sys.exit()

# Define a function that will retrieve the necessary information
# and put it in a dictionary.
def get_arguments_from_function(function):
    dfp = function.DoseFunctionParameters
    arg_dict = {}
    arg_dict['RoiName'] = function.ForRegionOfInterest.Name
    arg_dict['IsRobust'] = function.UseRobustness
    arg_dict['Weight'] = dfp.Weight
    if hasattr(dfp, "FunctionType"):
        arg_dict['FunctionType'] = dfp.FunctionType
        arg_dict['DoseLevel'] = dfp.DoseLevel
        if 'Eud' in dfp.FunctionType:
            arg_dict['EudParameterA'] = dfp.EudParameterA
        elif 'Dvh' in dfp.FunctionType:
            arg_dict['PercentVolume'] = dfp.PercentVolume
    elif hasattr(dfp, 'HighDoseLevel'):
        # Dose fall-off function does not have function type attribute.
        arg_dict['FunctionType'] = 'DoseFallOff'
        arg_dict['HighDoseLevel'] = dfp.HighDoseLevel
        arg_dict['LowDoseLevel'] = dfp.LowDoseLevel
        arg_dict['LowDoseDistance'] = dfp.LowDoseDistance
    elif hasattr(dfp, 'PercentStdDeviation'):
        # Uniformity constraint does not have function type.
        arg_dict['FunctionType'] = 'UniformityConstraint'
        arg_dict['PercentStdDeviation'] = dfp.PercentStdDeviation
    else:
        # Unknown function type, raise exception.
        raise ('Unknown function type')
    return arg_dict

# Define a function that will use information in arg_dict to set
# optimization function parameters.
def set_function_arguments(function, arg_dict):
    dfp = function.DoseFunctionParameters
    dfp.Weight = arg_dict['Weight']
    if arg_dict['FunctionType'] == 'DoseFallOff':
        dfp.HighDoseLevel = arg_dict['HighDoseLevel']
        dfp.LowDoseLevel = arg_dict['LowDoseLevel']
        dfp.LowDoseDistance = arg_dict['LowDoseDistance']
    elif arg_dict['FunctionType'] == 'UniformityConstraint':
        dfp.PercentStdDeviation = arg_dict['PercentStdDeviation']
    else:
        dfp.DoseLevel = arg_dict['DoseLevel']
        if 'Eud' in dfp.FunctionType:
            dfp.EudParameterA = arg_dict['EudParameterA']
        elif 'Dvh' in dfp.FunctionType:
            dfp.PercentVolume = arg_dict['PercentVolume']

from Helpers import ClearObjectiveFunctions
if loadObjectiveFunctionsViewModel.DoesClearObjectiveFunctions:
    ClearObjectiveFunctions(plan)

with CompositeAction('Load Objective Functions'):

    # Add optimization functions to the new plan.
    po = plan.PlanOptimizations[0]
    if SizeOfIterator(po.OptimizedBeamSets) == 2:
        hasMultipleBeamSets = True
    else:
        hasMultipleBeamSets = False

    for r in rois:
        argumentsList = r.ObjectiveFunctionArguments
        for a in argumentsList:
            arg_dict = json.loads(a)
            description = GetObjectiveFunctionDescription(arg_dict)
            print arg_dict['IsCombined'], arg_dict['PlanLabel'], arg_dict['FunctionType'], description

            if arg_dict['FunctionType'] == 'UniformEud':
                arg_dict['FunctionType'] = 'TargetEud'
            if not hasMultipleBeamSets:
                f = po.AddOptimizationFunction(FunctionType=arg_dict['FunctionType'],
                        RoiName=arg_dict['RoiName'], IsConstraint=arg_dict['IsConstraint'],
                        IsRobust=arg_dict['IsRobust'])
            else:
                if arg_dict['IsCombined']:
                    restrictToBeamSet = None
                else:
                    restrictToBeamSet = arg_dict['PlanLabel']
                    
                f = po.AddOptimizationFunction(FunctionType=arg_dict['FunctionType'],
                        RoiName=arg_dict['RoiName'], IsConstraint=arg_dict['IsConstraint'],
                        IsRobust=arg_dict['IsRobust'], RestrictToBeamSet=restrictToBeamSet)

            set_function_arguments(f, arg_dict)












