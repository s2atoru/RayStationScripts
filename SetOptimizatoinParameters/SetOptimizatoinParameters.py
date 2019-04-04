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

from Helpers import SetMaxArcMu

plan = get_current("Plan")

#Initial settings
maxNumberOfIterations = 40
iterationsInPreparationsPhase = 20
computeFinalDose = True
constrainMaxMu = True
maxMuPerFxPerBeam = 250

settingParameters = SettingParameters()

settingParameters.MaxNumberOfIterations = maxNumberOfIterations
settingParameters.IterationsInPreparationsPhase = iterationsInPreparationsPhase
settingParameters.ComputeFinalDose = computeFinalDose
settingParameters.MaxMuPerFxPerBeam = maxMuPerFxPerBeam
settingParameters.ConstrainMaxMu = constrainMaxMu 

#print("{0}, {1}, {2}, {3}, {4}".format(settingParameters.MaxNumberOfIterations, settingParameters.IterationsInPreparationsPhase, settingParameters.ComputeFinalDose, settingParameters.IsValid, settingParameters.CanSetParameters))

mainWindow = MainWindow(settingParameters)
mainWindow.ShowDialog()

maxNumberOfIterations = settingParameters.MaxNumberOfIterations
iterationsInPreparationsPhase = settingParameters.IterationsInPreparationsPhase
computeFinalDose = settingParameters.ComputeFinalDose
constrainMaxMu = settingParameters.ConstrainMaxMu
maxMuPerFxPerBeam = settingParameters.MaxMuPerFxPerBeam

canSetParameters = settingParameters.CanSetParameters
isValid = settingParameters.IsValid

print 'Setting parameters'
print maxNumberOfIterations, iterationsInPreparationsPhase, computeFinalDose, constrainMaxMu, maxMuPerFxPerBeam, canSetParameters, isValid

#message = "{0}, {1}, {2}, {3}, {4}".format(maxNumberOfIterations, iterationsInPreparationsPhase, computeFinalDose, isValid, canSetParameters)
#MessageBox.Show(message)

def SetOptimizatonParameters():
    print("In SetOptimizatonParameters")
    print("{0}, {1}, {2}, {3}, {4}, {5}".format(maxNumberOfIterations, iterationsInPreparationsPhase, computeFinalDose, isValid, canSetParameters, maxMuPerFxPerBeam))
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
        
        treatmentSetupSettings = optimization.OptimizationParameters.TreatmentSetupSettings
        if constrainMaxMu:
            message = 'Set Max MU for all Arc beams as {0}'.format(maxMuPerFxPerBeam)
            message += '\n'
            message += 'CreateDualArcs -> False and BurstGantrySpacing -> None'
            MessageBox.Show(message)
        else:
            MessageBox.Show('Unchecked "Limit MU Apply" for all arc beams')

        for setting in treatmentSetupSettings:
            for beamSetting in setting.BeamSettings:
                if not beamSetting.ForBeam.DeliveryTechnique == 'Arc':
                    break
                
                if constrainMaxMu:
                    SetMaxArcMu(beamSetting, maxMuPerFxPerBeam)
                else:
                    SetMaxArcMu(beamSetting, None)
                
with CompositeAction('Plan optimization settings ({})'.format(plan.Name)):
    if(canSetParameters):
        #print("Perform Optimization settings")
        SetOptimizatonParameters()

    # CompositeAction ends 
#pass

