from connect import *

import sys, math, wpf, os
from System.Windows import MessageBox

import clr
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

dllsPath = os.environ["USERPROFILE"]
dllsPath = dllsPath + r"\Desktop\RayStationScripts\Dlls"
print(dllsPath)
sys.path.append(dllsPath)

clr.AddReference("OptimizatoinSettings")

from OptimizatoinSettings.Models import SettingParameters
from OptimizatoinSettings.Views import MainWindow

plan = get_current("Plan")

maxNumberOfIterations = 40
iterationsInPreparationsPhase = 20
computeFinalDose = True

settingParameters = SettingParameters()

settingParameters.MaxNumberOfIterations = maxNumberOfIterations
settingParameters.IterationsInPreparationsPhase = iterationsInPreparationsPhase
settingParameters.ComputeFinalDose = computeFinalDose

#print("{0}, {1}, {2}, {3}, {4}".format(settingParameters.MaxNumberOfIterations, settingParameters.IterationsInPreparationsPhase, settingParameters.ComputeFinalDose, settingParameters.IsValid, settingParameters.CanSetParameters))

mainWindow = MainWindow(settingParameters)
mainWindow.ShowDialog()

maxNumberOfIterations = settingParameters.MaxNumberOfIterations
iterationsInPreparationsPhase = settingParameters.IterationsInPreparationsPhase
computeFinalDose = settingParameters.ComputeFinalDose
canSetParameters = settingParameters.CanSetParameters
isValid = settingParameters.IsValid

#message = "{0}, {1}, {2}, {3}, {4}".format(maxNumberOfIterations, iterationsInPreparationsPhase, computeFinalDose, isValid, canSetParameters)
#MessageBox.Show(message)

def SetOptimizatonParameters():
#    print("In SetOptimizatonParameters")
#    print("{0}, {1}, {2}, {3}, {4}".format(maxNumberOfIterations, iterationsInPreparationsPhase, computeFinalDose, isValid, canSetParameters))
    for optimization in plan.PlanOptimizations:
        optimization.OptimizationParameters.Algorithm.MaxNumberOfIterations = maxNumberOfIterations
        optimization.OptimizationParameters.DoseCalculation.IterationsInPreparationsPhase = iterationsInPreparationsPhase
        optimization.OptimizationParameters.DoseCalculation.ComputeFinalDose = computeFinalDose

        #optimizedBeamSets = []
        for optimizedBeamSet in optimization.OptimizedBeamSets:
            #optimizedBeamSets.append(optimizedBeamSet.DicomPlanLabel)
            dicomPlanLabel = optimizedBeamSet.DicomPlanLabel
            message = "Set Optimization Parameters\n"
            #message += "Beam Sets: " + ",".join(optimizedBeamSets) + "\n"
            message += "Beam Set: {0}".format(dicomPlanLabel) + "\n"
            message += "Max number of iterations: {0}\n".format(maxNumberOfIterations)
            message += "Iterations before conversion: {0}\n".format(iterationsInPreparationsPhase)
            message += "Compute final dose: {0}\n".format(str(computeFinalDose))
       
            MessageBox.Show(message)

with CompositeAction('Plan optimization settings ({})'.format(plan.Name)):
    if(canSetParameters):
        #print("Perform Optimization settings")
        SetOptimizatonParameters()

    # CompositeAction ends 
#pass
