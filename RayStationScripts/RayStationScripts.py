import clr
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

import sys, os

from System.Collections.Generic import List

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"
dllsPath = RayStationScriptsPath + "Dlls"
print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

from Helpers import MarginDict

marginDict = MarginDict([4]*8)

print marginDict

#sys.path.append(r"C:\Users\satoru\Source\Repos\RayStationScripts\OptimizatoinSettings\bin\Debug")
#clr.AddReference("OptimizatoinSettings")

#from OptimizatoinSettings.Models import SettingParameters
#from OptimizatoinSettings.Views import MainWindow

#settingParameters = SettingParameters()

#settingParameters.MaxNumberOfIterations = 50

#print("{0}, {1}, {2}, {3}, {4}".format(settingParameters.MaxNumberOfIterations, settingParameters.IterationsInPreparationsPhase, settingParameters.ComputeFinalDose, settingParameters.IsValid, settingParameters.CanSetParameters))

#settingParameters.MaxNumberOfIterations = 10

#print("{0}, {1}, {2}, {3}, {4}".format(settingParameters.MaxNumberOfIterations, settingParameters.IterationsInPreparationsPhase, settingParameters.ComputeFinalDose, settingParameters.IsValid, settingParameters.CanSetParameters))

#mainWindow = MainWindow(settingParameters)

#mainWindow.ShowDialog()

#print("{0}, {1}, {2}, {3}, {4}".format(settingParameters.MaxNumberOfIterations, settingParameters.IterationsInPreparationsPhase, settingParameters.ComputeFinalDose, settingParameters.IsValid, settingParameters.CanSetParameters))

print('Hello world')