import clr
clr.AddReference("PresentationFramework")
clr.AddReference("PresentationCore")

import sys, os
import json

from System.Collections.Generic import List

RayStationScriptsPath = os.environ["USERPROFILE"] + r"\DeskTop\RayStationScripts" + "\\"

dllsPath = RayStationScriptsPath + "Dlls"
print(dllsPath)
sys.path.append(dllsPath)

scriptsPath = RayStationScriptsPath + "Scripts"
print(scriptsPath)
sys.path.append(scriptsPath)

print ','.join(sys.path)

roiFormulasPath = RayStationScriptsPath + "RoiFormulas"

clr.AddReference("RoiFormulaMaker")
from RoiFormulaMaker.Views import MainWindow

structureNames = List[str](['PTV', 'Rectum', 'Bladder', 'FemoralHeads'])

structureDesigns = List[object]()

mainWindow = MainWindow(structureNames, structureDesigns, roiFormulasPath)
mainWindow.ShowDialog();

#Python JSON encoder and decoder
#https://docs.python.org/2.7/library/json.html
jsonList = []
for s in structureDesigns:
    print(s,s.ToJson())
    #To dictionary
    jsonList.append(json.loads(s.ToJson()))

from StringIO import StringIO
io = StringIO()
#JSON list to JSON text
json.dump(jsonList, io)
print io.getvalue()

#ringRoiParameters = RingRoiParameters();
#marginAddedRoiParameters = MarginAddedRoiParameters();

#print marginAddedRoiParameters.ToJson();


#from Helpers import MarginDict

#marginDict = MarginDict([4]*8)

#print marginDict

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
pass